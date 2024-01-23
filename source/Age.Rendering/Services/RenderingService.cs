using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Display;
using Age.Rendering.Drawing;
using Age.Rendering.Enums;
using Age.Rendering.Vulkan;
using Age.Rendering.Vulkan.Handlers;
using Age.Rendering.Vulkan.Uniforms;

namespace Age.Rendering.Services;

public class RenderingService : IDisposable
{
    private readonly IndexBufferHandler indexBuffer;
    private readonly VulkanRenderer     renderer;
    private readonly Shader             shader;

    private readonly Dictionary<Texture, UniformSet>              textureSets   = [];
    private readonly Dictionary<DrawCommand, VertexBufferHandler> vertexBuffers = [];

    private bool disposed;

    public RenderingService(VulkanRenderer renderer)
    {
        this.indexBuffer = renderer.CreateIndexBuffer([0u, 1, 2, 0, 2, 3]);
        this.renderer    = renderer;
        this.shader      = new() { Handler = renderer.CreateShader() };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.renderer.WaitIdle();

                foreach (var uniformSet in this.textureSets.Values)
                {
                    this.renderer.DestroyUniformSet(uniformSet);
                }

                foreach (var vertexBuffer in this.vertexBuffers.Values)
                {
                    this.renderer.DestroyVertexBuffer(vertexBuffer);
                }

                this.renderer.DestroyShader(this.shader);
                this.renderer.DestroyIndexBuffer(this.indexBuffer);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    private void Render(Window window, Element element)
    {
        IndexBufferHandler?  lastIndexBuffer  = default;
        VertexBufferHandler? lastVertexBuffer = default;
        UniformSet?          lastUniformSet   = default;

        var windowSize = window.ClientSize;

        foreach (var command in element.Commands)
        {
            switch (command)
            {
                case RectDrawCommand rectDrawCommand:
                    {
                        if (!this.textureSets.TryGetValue(rectDrawCommand.Texture, out var uniformSet))
                        {
                            var uniform = new CombinedImageSamplerUniform
                            {
                                Binding = 0,
                                Images  =
                                [
                                    new()
                                    {
                                        Sampler = rectDrawCommand.Texture.Sampler.Handler,
                                        Texture = rectDrawCommand.Texture.Handler,
                                    }
                                ]
                            };

                            this.textureSets[rectDrawCommand.Texture] = uniformSet = this.renderer.CreateUniformSet([uniform], this.shader.Handler);
                        }

                        var rect = rectDrawCommand.Rect;

                        if (!this.vertexBuffers.TryGetValue(command, out var vertexBuffer))
                        {
                            this.vertexBuffers[command] = vertexBuffer = this.renderer.CreateVertexBuffer(new Vertex[4]);
                        }

                        var x1 = rect.Position.X / windowSize.Width;
                        var x2 = (rect.Position.X + rect.Size.Width) / windowSize.Width;
                        var y1 = -rect.Position.Y / windowSize.Height;
                        var y2 = (-rect.Position.Y + rect.Size.Height) / windowSize.Height;

                        var p1 = new Point<float>(x1 * 2 - 1, y1 * 2 - 1);
                        var p2 = new Point<float>(x2 * 2 - 1, y1 * 2 - 1);
                        var p3 = new Point<float>(x2 * 2 - 1, y2 * 2 - 1);
                        var p4 = new Point<float>(x1 * 2 - 1, y2 * 2 - 1);

                        var vertices = new Vertex[4]
                        {
                            new(p1, default, new(0, 0)),
                            new(p2, default, new(1, 0)),
                            new(p3, default, new(1, 1)),
                            new(p4, default, new(0, 1)),
                        };

                        if (vertexBuffer != lastVertexBuffer)
                        {
                            this.renderer.UpdateVertexBuffer(vertexBuffer, vertices);
                            this.renderer.BindVertexBuffer(vertexBuffer);

                            lastVertexBuffer = vertexBuffer;
                        }

                        if (uniformSet != lastUniformSet)
                        {
                            this.renderer.BindUniformSet(uniformSet);

                            lastUniformSet = uniformSet;
                        }

                        if (this.indexBuffer != lastIndexBuffer)
                        {
                            this.renderer.BindIndexBuffer(this.indexBuffer);

                            lastIndexBuffer = this.indexBuffer;
                        }

                        this.renderer.DrawIndexed(this.indexBuffer);

                        break;
                    }
                default:
                    throw new Exception();
            }
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Texture CreateTexture(Image image, TextureType textureType, Sampler sampler)
    {
        var textureCreate = new TextureCreate
        {
            Data        = image.Pixels,
            Depth       = 1,
            Width       = image.Width,
            Height      = image.Height,
            Format      = default,
            TextureType = TextureType.T2D,
        };

        var textureData = this.renderer.CreateTexture(textureCreate);

        return new()
        {
            Depth       = 1,
            Handler     = textureData,
            Image       = image,
            Sampler     = sampler,
            TextureType = textureType,
        };
    }

    public Sampler CreateSampler() =>
        new() { Handler = this.renderer.CreateSampler() };

    public void Render(Window window)
    {
        this.renderer.SetViewport(window.Context);
        this.renderer.BeginRenderPass(window.Context);

        this.renderer.BindPipeline(this.shader.Handler);

        foreach (var element in window.Content.Enumerate<Element>())
        {
            this.Render(window, element);
        }

        this.renderer.EndRenderPass();
    }

    public void FreeSampler(Sampler sampler)
    {
        this.renderer.WaitIdle();
        this.renderer.DestroySampler(sampler.Handler);
    }

    public void FreeTexture(Texture texture)
    {
        this.renderer.WaitIdle(); // TODO find better solution
        this.textureSets.Remove(texture);
        this.renderer.DestroyTexture(texture.Handler);
    }
}
