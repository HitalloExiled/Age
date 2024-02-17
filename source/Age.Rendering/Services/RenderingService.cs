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

    private RenderPass colorPass;
    private RenderPass wireframePass;

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
                        #if DRAW_WIREFRAME
                        FinalLayout    = VkImageLayout.ColorAttachmentOptimal,
                        #else
                        FinalLayout    = VkImageLayout.PresentSrcKHR,
                        #endif
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StoreOp        = VkAttachmentStoreOp.Store,
                        StencilLoadOp  = VkAttachmentLoadOp.Clear,
                        StencilStoreOp = VkAttachmentStoreOp.Store,
                    },
                }
            ],
        };

        var wireframePassCreateInfo = new RenderPass.CreateInfo
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
                        InitialLayout  = VkImageLayout.ColorAttachmentOptimal,
                        FinalLayout    = VkImageLayout.PresentSrcKHR,
                        LoadOp         = VkAttachmentLoadOp.Load,
                        StoreOp        = VkAttachmentStoreOp.Store,
                        StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                        StencilStoreOp = VkAttachmentStoreOp.DontCare,
                    },
                }
            ],
        };

        this.colorPass            = renderer.Context.CreateRenderPass(colorPassCreateInfo);
        this.wireframePass        = renderer.Context.CreateRenderPass(wireframePassCreateInfo);
        this.indexBuffer          = renderer.CreateIndexBuffer([0u, 1, 2, 0, 2, 3]);
        this.wireframeIndexBuffer = renderer.CreateIndexBuffer([0u, 1, 1, 2, 2, 3, 3, 0, 0, 2]);
        this.renderer             = renderer;
        this.diffuseShader        = renderer.CreateShader<CanvasShader, Vertex>(this.colorPass);
        this.wireframeShader      = renderer.CreateShader<WireframeShader, Vertex>(this.wireframePass);

        this.renderer.Context.SwapchainRecreated += () =>
        {
            this.colorPass.Dispose();
            this.wireframePass.Dispose();

            this.colorPass     = renderer.Context.CreateRenderPass(colorPassCreateInfo);
            this.wireframePass = renderer.Context.CreateRenderPass(wireframePassCreateInfo);

            this.RequestDraw();
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.renderer.Context.DefferedDispose(this.colorPass);
                this.renderer.Context.DefferedDispose(this.wireframePass);
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

    private void Render(IWindow window, Element element, bool wireframe)
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

                        var hashcode = wireframe.GetHashCode() ^ windowSize.GetHashCode() ^ rectDrawCommand.Rect.GetHashCode();

                        if (!this.vertexBuffers.TryGetValue(hashcode, out var vertexBuffer))
                        {
                            var rect = rectDrawCommand.Rect;

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

                        var indexBuffer = wireframe ? this.wireframeIndexBuffer : this.indexBuffer;

                        if (wireframe)
                        {
                            this.renderer.BindIndexBuffer(this.wireframeIndexBuffer);
                        }
                        else
                        {
                            this.renderer.BindIndexBuffer(this.indexBuffer);
                        }

                        this.renderer.DrawIndexed(indexBuffer);

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
        var passes = new[]
        {
            (this.diffuseShader,   this.colorPass),
            #if DRAW_WIREFRAME
            (this.wireframeShader, this.wireframePass),
            #endif
        };

        foreach (var (shader, renderPass) in passes)
        {
            this.renderer.BeginRenderPass(renderPass, window.Surface.Swapchain.Extent, window.Surface.CurrentBuffer);
            this.renderer.SetViewport(window.Surface);

            this.renderer.BindPipeline(shader);

            foreach (var element in window.Content.Enumerate<Element>())
            {
                this.Render(window, element, shader == this.wireframeShader);
            }

            this.renderer.EndRenderPass();
        }
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
