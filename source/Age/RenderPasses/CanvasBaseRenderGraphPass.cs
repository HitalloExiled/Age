using System.Runtime.CompilerServices;
using Age.Numerics;
using Age.Commands;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using Age.Rendering.Shaders.Canvas;
using Age.Elements;
using Age.Rendering.Uniforms;
using ThirdParty.Vulkan.Enums;

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
        this.CommandBuffer.BindVertexBuffer(vertexBuffer);
        this.CommandBuffer.BindIndexBuffer(indexBuffer);
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

        var viewport      = this.Window.ClientSize;
        var viewportFloat = viewport.Cast<float>();
        var extent        = Unsafe.As<Size<uint>, VkExtent2D>(ref viewport);

        Span<VkClearValue> clearValues = stackalloc VkClearValue[2];

        clearValues[0].Color.Float32[0] = this.ClearColor.R;
        clearValues[0].Color.Float32[1] = this.ClearColor.G;
        clearValues[0].Color.Float32[2] = this.ClearColor.B;
        clearValues[0].Color.Float32[3] = this.ClearColor.A;
        clearValues[1].DepthStencil.Depth = 1;

        this.CommandBuffer.SetViewport(extent);
        this.CommandBuffer.BeginRenderPass(this.RenderPass, this.Framebuffer, [clearValues[0]]);

        Layer? layer = null;

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
                            if (rectCommand.Layer != null && rectCommand.Layer != layer)
                            {
                                var constant = new CanvasShader.PushConstant
                                {
                                    Rect      = new(rectCommand.Layer.Texture.Image.Extent.Width, rectCommand.Layer.Texture.Image.Extent.Height, 0, 0),
                                    Transform = rectCommand.Layer.Owner.TransformCache,
                                    UV        = UVRect.Normalized,
                                    Viewport  = viewportFloat,
                                };

                                this.DrawStencilLayer(constant, rectCommand.Layer, resource.VertexBuffer, resource.IndexBuffer);

                                this.CommandBuffer.BindShader(resource.Shader);
                            }

                            layer = rectCommand.Layer;

                            this.ExecuteCommand(resource, rectCommand, viewportFloat, entry.Transform);

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        this.CommandBuffer.EndRenderPass();

        this.AfterExecute();
    }

    protected abstract void ExecuteCommand(RenderResources resource, RectCommand command, in Size<float> viewport, in Matrix3x2<float> transform);
}
