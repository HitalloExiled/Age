using Age.Commands;
using Age.Elements;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Shaders;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;
using System.Runtime.CompilerServices;
using Age.Resources;
using Age.Core.Extensions;

namespace Age.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass(VulkanRenderer renderer, Window window) : RenderGraphPass(renderer, window)
{
    private Size<float>? previousViewport;

    protected ResourceCache<Texture2D, UniformSet> UniformSets { get; } = new();

    protected UniformSet? LastUniformSet { get; set; }

    protected abstract CanvasStencilMaskShader CanvasStencilMaskShader { get; }
    protected abstract Color                   ClearColor              { get; }
    protected abstract CommandBuffer           CommandBuffer           { get; }
    protected abstract Framebuffer             Framebuffer             { get; }
    protected abstract RenderPipelines[]       Pipelines               { get; }
    protected abstract PipelineVariant         PipelineVariants        { get; }

    private void ClearStencilBuffer(in VkExtent2D extent) =>
        this.ClearStencilBufferAttachment(extent, 0);

    private unsafe void ClearStencilBufferAttachment(in VkExtent2D extent, uint value)
    {
        var attachment = new VkClearAttachment
        {
            AspectMask = VkImageAspectFlags.Stencil,
            ClearValue =
            {
                DepthStencil =
                {
                    Depth   = 1,
                    Stencil = value,
                }
            },
            ColorAttachment = 0,
        };

        var rect = new VkClearRect
        {
            Rect           = new() { Extent = extent },
            BaseArrayLayer = 0,
            LayerCount     = 1,
        };

        this.CommandBuffer.ClearAttachments([attachment], [rect]);
    }

    private void DrawStencilBuffer(in Size<float> viewport, StencilLayer stencilLayer, IndexBuffer indexBuffer)
    {
        stencilLayer.Update();

        if (!this.UniformSets.TryGetValue(stencilLayer.TextureMap.Texture, out var uniformSet))
        {
            var stencil = new CombinedImageSamplerUniform
            {
                Binding     = 0,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                Image       = stencilLayer.TextureMap.Texture.Image,
                ImageView   = stencilLayer.TextureMap.Texture.ImageView,
                Sampler     = stencilLayer.TextureMap.Texture.Sampler,
            };

            this.UniformSets.Set(stencilLayer.TextureMap.Texture, uniformSet = new UniformSet(this.CanvasStencilMaskShader, [stencil]));
        }

        var constant = new CanvasShader.PushConstant
        {
            Size      = stencilLayer.Size.Cast<float>(),
            Transform = stencilLayer.Transform,
            UV        = stencilLayer.TextureMap.UV,
            Viewport  = viewport,
        };

        this.CommandBuffer.BindShader(this.CanvasStencilMaskShader);
        this.CommandBuffer.BindUniformSet(this.LastUniformSet = uniformSet);
        this.CommandBuffer.PushConstant(this.CanvasStencilMaskShader, constant);
        this.CommandBuffer.DrawIndexed(indexBuffer);
    }

    private void FillStencilBuffer(in VkExtent2D extent) =>
        this.ClearStencilBufferAttachment(extent, 1);

    protected static RenderPass CreateRenderPass(VkFormat colorAttachmentFormat, VkImageLayout colorAttachmentFinalLayout)
    {
        var createInfo = new RenderPassCreateInfo
        {
            SubPasses =
            [
                new()
                {
                    PipelineBindPoint = VkPipelineBindPoint.Graphics,
                    ColorAttachments  =
                    [
                        new()
                        {
                            Color = new()
                            {
                                Format         = colorAttachmentFormat,
                                Samples        = VkSampleCountFlags.N1,
                                InitialLayout  = VkImageLayout.Undefined,
                                FinalLayout    = colorAttachmentFinalLayout,
                                LoadOp         = VkAttachmentLoadOp.Clear,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare
                            },
                        }
                    ],
                    DepthStencilAttachment = new()
                    {
                        Format         = VulkanRenderer.Singleton.StencilBufferFormat,
                        Samples        = VkSampleCountFlags.N1,
                        InitialLayout  = VkImageLayout.Undefined,
                        FinalLayout    = VkImageLayout.DepthStencilAttachmentOptimal,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StoreOp        = VkAttachmentStoreOp.Store,
                        StencilLoadOp  = VkAttachmentLoadOp.Clear,
                        StencilStoreOp = VkAttachmentStoreOp.DontCare
                    },
                },
            ],
            SubpassDependencies =
            [
                new()
                {
                    SrcSubpass      = VkConstants.VK_SUBPASS_EXTERNAL,
                    DstSubpass      = 0,
                    SrcStageMask    = VkPipelineStageFlags.EarlyFragmentTests | VkPipelineStageFlags.LateFragmentTests,
                    DstStageMask    = VkPipelineStageFlags.EarlyFragmentTests | VkPipelineStageFlags.LateFragmentTests,
                    SrcAccessMask   = VkAccessFlags.DepthStencilAttachmentWrite,
                    DstAccessMask   = VkAccessFlags.DepthStencilAttachmentWrite | VkAccessFlags.DepthStencilAttachmentRead,
                    DependencyFlags = default,
                },
                new()
                {
                    SrcSubpass      = VkConstants.VK_SUBPASS_EXTERNAL,
                    DstSubpass      = 0,
                    SrcStageMask    = VkPipelineStageFlags.ColorAttachmentOutput,
                    DstStageMask    = VkPipelineStageFlags.ColorAttachmentOutput,
                    SrcAccessMask   = default,
                    DstAccessMask   = VkAccessFlags.ColorAttachmentWrite | VkAccessFlags.ColorAttachmentRead,
                    DependencyFlags = default,
                }
            ]
        };

        return new(createInfo);
    }

    protected virtual void AfterExecute() { }

    protected virtual void BeforeExecute() { }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Disposed();
        }
    }

    protected abstract void ExecuteCommand(RenderPipelines resource, RectCommand command, in Size<float> viewport, in Transform2D transform);
    protected abstract void Disposed();

    public unsafe override void Execute()
    {
        var clientSize = this.Window.ClientSize;
        var viewport   = this.Window.ClientSize.Cast<float>();
        ref var extent = ref Unsafe.As<Size<uint>, VkExtent2D>(ref clientSize);

        this.BeforeExecute();

        Span<VkClearValue> clearValues  = stackalloc VkClearValue[2];

        clearValues[0].Color.Float32[0] = this.ClearColor.R;
        clearValues[0].Color.Float32[1] = this.ClearColor.G;
        clearValues[0].Color.Float32[2] = this.ClearColor.B;
        clearValues[0].Color.Float32[3] = this.ClearColor.A;
        clearValues[1].DepthStencil.Depth   = 1;
        clearValues[1].DepthStencil.Stencil = 1;

        this.CommandBuffer.SetViewport(extent);
        this.CommandBuffer.BeginRenderPass(this.RenderPass, this.Framebuffer, clearValues);

        if (this.previousViewport == viewport || !this.previousViewport.HasValue)
        {
            StencilLayer? previousLayer = null;

            foreach (var pipeline in this.Pipelines)
            {
                if (pipeline.Enabled)
                {
                    this.CommandBuffer.BindShader(pipeline.Shader);
                    this.CommandBuffer.BindVertexBuffer(pipeline.VertexBuffer);
                    this.CommandBuffer.BindIndexBuffer(pipeline.IndexBuffer);

                    foreach (var (command, transform) in this.Window.Tree.Get2DCommands())
                    {
                        if (this.PipelineVariants.HasAnyFlag(command.PipelineVariant))
                        {
                            if (!pipeline.IgnoreStencil && command.StencilLayer != previousLayer)
                            {
                                if (command.StencilLayer == null)
                                {
                                    this.FillStencilBuffer(extent);
                                }
                                else
                                {
                                    this.ClearStencilBuffer(extent);
                                    this.DrawStencilBuffer(viewport, command.StencilLayer, pipeline.IndexBuffer);

                                    this.CommandBuffer.BindShader(pipeline.Shader);
                                }

                                previousLayer = command.StencilLayer;
                            }

                            switch (command)
                            {
                                case RectCommand rectCommand:
                                    this.ExecuteCommand(pipeline, rectCommand, viewport, transform);

                                    break;
                                default:
                                    throw new InvalidOperationException($"Unsupported command type '{command.GetType().FullName}'.");
                            }
                        }
                    }
                }
            }
        }

        this.CommandBuffer.EndRenderPass();

        this.AfterExecute();

        this.previousViewport = viewport;
    }
}
