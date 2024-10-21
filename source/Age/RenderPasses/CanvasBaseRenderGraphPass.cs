using Age.Commands;
using Age.Elements;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Age.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass(VulkanRenderer renderer, Window window) : RenderGraphPass(renderer, window)
{
    protected ResourceCache<Texture, UniformSet> UniformSets { get; } = new();

    private Size<float>? previousViewport;

    protected abstract CanvasStencilMaskShader CanvasStencilMaskShader { get; }
    protected abstract Color                   ClearColor              { get; }
    protected abstract CommandBuffer           CommandBuffer           { get; }
    protected abstract Framebuffer             Framebuffer             { get; }
    protected abstract RenderPipelines[]       Pipelines               { get; }

    private unsafe void ClearStencilBufferAttachment(in VkExtent2D extent, uint value)
    {
        var clearValue = new VkClearValue();

        clearValue.DepthStencil.Depth   = 1;
        clearValue.DepthStencil.Stencil = value;

        var attachment = new VkClearAttachment
        {
            AspectMask      = VkImageAspectFlags.Stencil,
            ClearValue      = clearValue,
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

    private void ClearStencilBuffer(in VkExtent2D extent) =>
        this.ClearStencilBufferAttachment(extent, 0);

    private void FillStencilBuffer(in VkExtent2D extent) =>
        this.ClearStencilBufferAttachment(extent, 1);

    private void DrawStencilBuffer(in Size<float> viewport, StencilLayer stencilLayer, IndexBuffer indexBuffer)
    {
        stencilLayer.Update();

        if (!this.UniformSets.TryGetValue(stencilLayer.MappedTexture.Texture, out var uniformSet))
        {
            var stencil = new CombinedImageSamplerUniform
            {
                Binding     = 0,
                Texture     = stencilLayer.MappedTexture.Texture,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
            };

            this.UniformSets.Set(stencilLayer.MappedTexture.Texture, uniformSet = new UniformSet(this.CanvasStencilMaskShader, [stencil]));
        }

        var constant = new CanvasShader.PushConstant
        {
            Rect      = new(stencilLayer.Size.Cast<float>(), default),
            Transform = stencilLayer.Transform,
            UV        = stencilLayer.MappedTexture.UV,
            Viewport  = viewport,
        };

        this.CommandBuffer.BindShader(this.CanvasStencilMaskShader);
        this.CommandBuffer.BindUniformSet(uniformSet);
        this.CommandBuffer.PushConstant(this.CanvasStencilMaskShader, constant);
        this.CommandBuffer.DrawIndexed(indexBuffer);
    }

    protected virtual void AfterExecute() { }
    protected virtual void BeforeExecute() { }

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
        clearValues[1].DepthStencil.Depth = 1;

        this.CommandBuffer.SetViewport(extent);
        this.CommandBuffer.BeginRenderPass(this.RenderPass, this.Framebuffer, clearValues);

        if (this.previousViewport == viewport || !this.previousViewport.HasValue)
        {
            StencilLayer? previousLayer = null;

            foreach (var pipeline in this.Pipelines)
            {
                if (pipeline.Enabled)
                {
                    this.FillStencilBuffer(extent);

                    this.CommandBuffer.BindShader(pipeline.Shader);
                    this.CommandBuffer.BindVertexBuffer(pipeline.VertexBuffer);
                    this.CommandBuffer.BindIndexBuffer(pipeline.IndexBuffer);

                    foreach (var entry in this.Window.Tree.Enumerate2DCommands())
                    {
                        switch (entry.Command)
                        {
                            case RectCommand rectCommand:
                                if (!pipeline.IgnoreStencil && rectCommand.StencilLayer != previousLayer)
                                {
                                    if (rectCommand.StencilLayer != null)
                                    {
                                        this.ClearStencilBuffer(extent);
                                        this.DrawStencilBuffer(viewport, rectCommand.StencilLayer, pipeline.IndexBuffer);

                                        this.CommandBuffer.BindShader(pipeline.Shader);
                                    }
                                    else
                                    {
                                        this.FillStencilBuffer(extent);
                                    }
                                }

                                previousLayer = rectCommand.StencilLayer;

                                this.ExecuteCommand(pipeline, rectCommand, viewport, entry.Transform);

                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        this.CommandBuffer.EndRenderPass();

        this.AfterExecute();

        this.previousViewport = viewport;
    }

    protected abstract void ExecuteCommand(RenderPipelines resource, RectCommand command, in Size<float> viewport, in Transform2D transform);
}
