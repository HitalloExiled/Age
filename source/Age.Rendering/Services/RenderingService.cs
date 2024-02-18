#define DRAW_WIREFRAME
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using Age.Rendering.Enums;
using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan.Uniforms;
using Age.Rendering.Shaders;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Services;

public class RenderingService : IDisposable
{
    private readonly IndexBuffer    indexBuffer;
    private readonly IndexBuffer    wireframeIndexBuffer;
    private readonly VulkanRenderer renderer;
    private readonly Shader         diffuseShader;
    private readonly Shader         wireframeShader;

    private readonly Dictionary<Texture, UniformSet> textureSets   = [];
    private readonly Dictionary<int, VertexBuffer>   vertexBuffers = [];

    private RenderPass renderPass;

    private int  changes;
    private bool disposed;

    public RenderingService(VulkanRenderer renderer)
    {
        var colorPassCreateInfo = new RenderPass.CreateInfo
        {
            ColorAttachments =
            [
                new()
                {
                    Layout = VkImageLayout.ColorAttachmentOptimal,
                    Color  = new VkAttachmentDescription
                    {
                        Format         = renderer.Context.ScreenFormat,
                        Samples        = VkSampleCountFlags.N1,
                        InitialLayout  = VkImageLayout.Undefined,
                        FinalLayout    = VkImageLayout.PresentSrcKHR,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StoreOp        = VkAttachmentStoreOp.Store,
                        StencilLoadOp  = VkAttachmentLoadOp.Clear,
                        StencilStoreOp = VkAttachmentStoreOp.Store,
                    },
                }
            ],
        };

        this.renderPass           = renderer.Context.CreateRenderPass(colorPassCreateInfo);
        this.indexBuffer          = renderer.CreateIndexBuffer([0u, 1, 2, 0, 2, 3]);
        this.wireframeIndexBuffer = renderer.CreateIndexBuffer([0u, 1, 1, 2, 2, 3, 3, 0, 0, 2]);
        this.renderer             = renderer;
        this.diffuseShader        = renderer.CreateShader<CanvasShader, Vertex>(this.renderPass);
        this.wireframeShader      = renderer.CreateShader<WireframeShader, Vertex>(this.renderPass);

        this.renderer.Context.SwapchainRecreated += () =>
        {
            this.renderPass.Dispose();

            this.renderPass     = renderer.Context.CreateRenderPass(colorPassCreateInfo);

            this.RequestDraw();
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.renderer.Context.DefferedDispose(this.renderPass);
                this.renderer.Context.DefferedDispose(this.diffuseShader);
                this.renderer.Context.DefferedDispose(this.wireframeShader);
                this.renderer.Context.DefferedDispose(this.indexBuffer);
                this.renderer.Context.DefferedDispose(this.wireframeIndexBuffer);
                this.renderer.Context.DefferedDispose(this.vertexBuffers.Values);
                this.renderer.Context.DefferedDispose(this.textureSets.Values);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    private void Render(IWindow window, Element element)
    {
        VertexBuffer? lastVertexBuffer = default;
        UniformSet?   lastUniformSet   = default;

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
                                        Sampler = rectDrawCommand.Sampler,
                                        Texture = rectDrawCommand.Texture,
                                    }
                                ]
                            };

                            this.textureSets[rectDrawCommand.Texture] = uniformSet = this.renderer.CreateUniformSet([uniform], this.diffuseShader);
                        }

                        var hashcode = windowSize.GetHashCode() ^ rectDrawCommand.Rect.GetHashCode();

                        if (!this.vertexBuffers.TryGetValue(hashcode, out var vertexBuffer))
                        {
                            var rect = rectDrawCommand.Rect;

                            var x1 = rect.Position.X / windowSize.Width;
                            var x2 = (rect.Position.X + rect.Size.Width) / windowSize.Width;
                            var y1 = -rect.Position.Y / windowSize.Height;
                            var y2 = (-rect.Position.Y + rect.Size.Height) / windowSize.Height;

                            var p1 = new Vector3<float>(x1 * 2 - 1, y1 * 2 - 1, 1);
                            var p2 = new Vector3<float>(x2 * 2 - 1, y1 * 2 - 1, 1);
                            var p3 = new Vector3<float>(x2 * 2 - 1, y2 * 2 - 1, 1);
                            var p4 = new Vector3<float>(x1 * 2 - 1, y2 * 2 - 1, 1);

                            var vertices = new Vertex[4]
                            {
                                new(p1, new(0, 0)),
                                new(p2, new(1, 0)),
                                new(p3, new(1, 1)),
                                new(p4, new(0, 1)),
                            };

                            this.vertexBuffers[hashcode] = vertexBuffer = this.renderer.CreateVertexBuffer(vertices);
                        }

                        if (vertexBuffer != lastVertexBuffer)
                        {
                            this.renderer.BindVertexBuffer(vertexBuffer);

                            lastVertexBuffer = vertexBuffer;
                        }

                        if (uniformSet != lastUniformSet)
                        {
                            this.renderer.BindUniformSet(uniformSet);

                            lastUniformSet = uniformSet;
                        }

                        this.renderer.BindPipeline(this.diffuseShader);
                        this.renderer.BindIndexBuffer(this.indexBuffer);
                        this.renderer.DrawIndexed(this.indexBuffer);

#if DRAW_WIREFRAME
                        this.renderer.BindPipeline(this.wireframeShader);
                        this.renderer.BindIndexBuffer(this.wireframeIndexBuffer);
                        this.renderer.DrawIndexed(this.wireframeIndexBuffer);
#endif

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

    public Texture CreateTexture(Image image, TextureType textureType)
    {
        var textureCreate = new TextureCreate
        {
            Data        = image.Pixels,
            Depth       = 1,
            Width       = image.Width,
            Height      = image.Height,
            TextureType = textureType,
        };

        return this.renderer.CreateTexture(textureCreate);
    }

    public Sampler CreateSampler() =>
        new() { Value = this.renderer.CreateSampler() };

    public void DestroySampler(Sampler sampler) =>
        this.renderer.Context.DefferedDispose(sampler);

    public void Render(IWindow window)
    {
        this.renderer.BeginRenderPass(this.renderPass, window.Surface.Swapchain.Extent, window.Surface.CurrentBuffer);
        this.renderer.SetViewport(window.Surface);

        foreach (var element in window.Content.Enumerate<Element>())
        {
            this.Render(window, element);
        }

        this.renderer.EndRenderPass();
    }

    public void FreeTexture(Texture texture)
    {
        this.renderer.Context.DefferedDispose(texture);
        this.textureSets.Remove(texture);
    }

    public void RequestDraw() =>
        this.changes++;

    public void Render(IEnumerable<IWindow> windows)
    {
        if (this.changes > 0)
        {
            this.renderer.BeginFrame();

            foreach (var window in windows)
            {
                if (window.Visible && !window.Minimized && !window.Closed)
                {
                    window.Content.Update();

                    this.Render(window);
                }
            }

            this.renderer.EndFrame();

            this.changes--;
        }
    }
}
