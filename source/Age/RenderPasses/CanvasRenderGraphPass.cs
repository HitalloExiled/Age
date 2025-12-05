using Age.Core.Extensions;
using Age.Commands;
using Age.Elements;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Shaders;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;
using System.Runtime.CompilerServices;

namespace Age.RenderPasses;

public abstract partial class CanvasRenderGraphPass(VulkanRenderer renderer, Window window) : RenderGraphPass(renderer, window)
{
    private readonly Stack<StencilLayer> stencilStack = [];

    protected ResourceCache<Texture, UniformSet> UniformSets { get; } = new();

    protected UniformSet? LastUniformSet { get; set; }

    protected abstract CanvasStencilMaskShader CanvasStencilEraserShader { get; }
    protected abstract CanvasStencilMaskShader CanvasStencilWriterShader { get; }
    protected abstract Color                   ClearColor                { get; }
    protected abstract CommandBuffer           CommandBuffer             { get; }
    protected abstract CommandFilter           CommandFilter             { get; }
    protected abstract Framebuffer             Framebuffer               { get; }
    protected abstract RenderPipelines[]       Pipelines                 { get; }

    private void ClearStencilBuffer(in VkExtent2D extent) =>
        this.ClearStencilBufferAttachment(extent, 0);

    private void ClearStencilBufferAttachment(in VkExtent2D extent, uint value)
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
            Rect =
            {
                Extent = extent
            },
            BaseArrayLayer = 0,
            LayerCount     = 1,
        };

        this.CommandBuffer.ClearAttachments([attachment], [rect]);
    }

    private void EraseStencil(RenderPipelines pipeline, StencilLayer stencilLayer, in Size<uint> viewport) =>
        this.SetStencil(this.CanvasStencilEraserShader, pipeline, stencilLayer, viewport);

    private void FillStencilBuffer(in VkExtent2D extent) =>
        this.ClearStencilBufferAttachment(extent, 1);

    private void WriteStencil(RenderPipelines pipeline, StencilLayer stencilLayer, in Size<uint> viewport) =>
        this.SetStencil(this.CanvasStencilWriterShader, pipeline, stencilLayer, viewport);

    private void SetStencil(CanvasStencilMaskShader shader, RenderPipelines pipeline, StencilLayer stencilLayer, in Size<uint> viewport)
    {
        this.CommandBuffer.BindShader(shader);

        var constant = new CanvasShader.PushConstant
        {
            Size      = stencilLayer.Size,
            Transform = stencilLayer.Transform,
            Border    = stencilLayer.Border,
            Viewport  = viewport,
            UV        = UVRect.Normalized,
        };

        this.CommandBuffer.PushConstant(shader, constant);
        this.CommandBuffer.DrawIndexed(pipeline.IndexBuffer);
    }

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
                        Format         = (VkFormat)VulkanRenderer.Singleton.StencilBufferFormat,
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

    protected abstract void ExecuteCommand(RenderPipelines resource, RectCommand command, in Size<uint> viewport);
    protected abstract void Disposed();

    public unsafe override void Execute()
    {
        var viewport   = this.Window.Size;
        ref var extent = ref Unsafe.As<Size<uint>, VkExtent2D>(ref viewport);

        this.BeforeExecute();

        Span<VkClearValue> clearValues = stackalloc VkClearValue[2];

        clearValues[0].Color.Float32[0] = this.ClearColor.R;
        clearValues[0].Color.Float32[1] = this.ClearColor.G;
        clearValues[0].Color.Float32[2] = this.ClearColor.B;
        clearValues[0].Color.Float32[3] = this.ClearColor.A;
        clearValues[1].DepthStencil.Depth = 1;

        this.CommandBuffer.SetViewport(viewport);
        this.CommandBuffer.BeginRenderPass(this.Framebuffer.Extent, this.RenderPass, this.Framebuffer, clearValues);

        if (!this.Window.Surface.IsDirty)
        {
            this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, 0u);

            foreach (var pipeline in this.Pipelines)
            {
                if (pipeline.Enabled)
                {
                    this.CommandBuffer.BindShader(pipeline.Shader);
                    this.CommandBuffer.BindVertexBuffer(pipeline.VertexBuffer);
                    this.CommandBuffer.BindIndexBuffer(pipeline.IndexBuffer);

                    var previousDepth = 0u;

                    var commands = this.Window.RenderContext.UIBuffer.Commands;

                    for (var i = 0; i < commands.Length; i++)
                    {
                        var command = commands[i];

                        if (this.CommandFilter.HasAnyFlag(command.CommandFilter))
                        {
                            if (!pipeline.IgnoreStencil && (!this.stencilStack.TryPeek(out var previousLayer) || command.StencilLayer != previousLayer))
                            {
                                if (command.StencilLayer is StencilLayer currentLayer)
                                {
                                    var currentDepth = (uint)(currentLayer.Depth + 1);

                                    while (this.stencilStack.Count > currentDepth)
                                    {
                                        var layer = this.stencilStack.Pop();

                                        this.EraseStencil(pipeline, layer, viewport);
                                    }

                                    if (currentDepth >= previousDepth)
                                    {
                                        this.WriteStencil(pipeline, currentLayer, viewport);

                                        if (currentDepth > previousDepth)
                                        {
                                            this.stencilStack.Push(currentLayer);
                                        }
                                    }

                                    this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, currentDepth);

                                    previousDepth = currentDepth;

                                    this.CommandBuffer.BindShader(pipeline.Shader);
                                }
                                else if (previousDepth > 0)
                                {
                                    this.stencilStack.Clear();
                                    this.ClearStencilBuffer(extent);
                                    this.CommandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, previousDepth = 0u);
                                    this.CommandBuffer.BindShader(pipeline.Shader);
                                }
                            }

                            switch (command)
                            {
                                case RectCommand rectCommand:
                                    this.ExecuteCommand(pipeline, rectCommand, viewport);

                                    break;
                                default:
                                    throw new InvalidOperationException($"Unsupported command type '{command.GetType().FullName}'.");
                            }
                        }
                    }

                    this.stencilStack.Clear();
                }
            }
        }

        this.CommandBuffer.EndRenderPass();

        this.AfterExecute();
    }
}
