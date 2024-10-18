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

namespace Age.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass(VulkanRenderer renderer, Window window) : RenderGraphPass(renderer, window)
{
    protected abstract CanvasStencilMaskShader CanvasStencilMaskShader { get; }
    protected abstract Color                   ClearColor              { get; }
    protected abstract CommandBuffer           CommandBuffer           { get; }
    protected abstract Framebuffer             Framebuffer             { get; }
    protected abstract RenderResources[]       Resources               { get; }

    public Dictionary<int, UniformSet> UniformSets { get; } = [];

    private void DrawStencilLayer(in CanvasShader.PushConstant constant, Layer layer, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
    {
        var hashcode = layer.Texture.GetHashCode();

        if (!this.UniformSets.TryGetValue(hashcode, out var uniformSet))
        {
            var stencil = new CombinedImageSamplerUniform
            {
                Binding     = 0,
                Texture     = layer.Texture,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
            };

            this.UniformSets[hashcode] = uniformSet = new UniformSet(this.CanvasStencilMaskShader, [stencil]);
        }

        this.CommandBuffer.BindShader(this.CanvasStencilMaskShader);
        this.CommandBuffer.BindUniformSet(uniformSet);
        this.CommandBuffer.PushConstant(this.CanvasStencilMaskShader, constant);
        this.CommandBuffer.DrawIndexed(indexBuffer);
    }

    protected override void Disposed()
    {
        foreach (var uniformSet in this.UniformSets.Values)
        {
            uniformSet.Dispose();
        }
    }

    protected virtual void AfterExecute() { }
    protected virtual void BeforeExecute() { }

    public unsafe override void Execute()
    {
        this.BeforeExecute();

        var viewport = this.Window.ClientSize.Cast<float>();
        var extent   = new VkExtent2D() { Width = (uint)viewport.Width, Height = (uint)viewport.Height };

        Span<VkClearValue> clearValues  = stackalloc VkClearValue[2];

        clearValues[0].Color.Float32[0] = this.ClearColor.R;
        clearValues[0].Color.Float32[1] = this.ClearColor.G;
        clearValues[0].Color.Float32[2] = this.ClearColor.B;
        clearValues[0].Color.Float32[3] = this.ClearColor.A;
        clearValues[1].DepthStencil.Depth   = 1;
        clearValues[1].DepthStencil.Stencil = 1;

        this.CommandBuffer.SetViewport(extent);
        this.CommandBuffer.BeginRenderPass(this.RenderPass, this.Framebuffer, clearValues);

        Layer? previousLayer = null;

        foreach (var resource in this.Resources)
        {
            if (resource.Enabled)
            {
                this.CommandBuffer.BindShader(resource.Shader);
                this.CommandBuffer.BindVertexBuffer(resource.VertexBuffer);
                this.CommandBuffer.BindIndexBuffer(resource.IndexBuffer);

                foreach (var entry in this.Window.Tree.Enumerate2DCommands())
                {
                    switch (entry.Command)
                    {
                        case RectCommand rectCommand:
                            if (rectCommand.Layer != previousLayer)
                            {
                                if (rectCommand.Layer != null)
                                {
                                    this.ClearStencilBuffer(extent, 0);

                                    var constant = new CanvasShader.PushConstant
                                    {
                                        Rect      = new(rectCommand.Layer.Size.Cast<float>(), default),
                                        Transform = rectCommand.Layer.Owner.TransformCache,
                                        UV        = UVRect.Normalized,
                                        Viewport  = viewport,
                                    };

                                    this.DrawStencilLayer(constant, rectCommand.Layer, resource.VertexBuffer, resource.IndexBuffer);

                                    this.CommandBuffer.BindShader(resource.Shader);
                                }
                                else
                                {
                                    this.ClearStencilBuffer(extent, 1);
                                }
                            }

                            previousLayer = rectCommand.Layer;

                            this.ExecuteCommand(resource, rectCommand, viewport, entry.Transform);

                            break;
                        default:
                            break;
                    }
                }

                this.ClearStencilBuffer(extent, 1);
            }
        }

        this.CommandBuffer.EndRenderPass();

        this.AfterExecute();
    }

    private unsafe void ClearStencilBuffer(VkExtent2D extent, uint value)
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

    protected abstract void ExecuteCommand(RenderResources resource, RectCommand command, in Size<float> viewport, in Transform2D transform);
}
