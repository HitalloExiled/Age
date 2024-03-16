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
using Age.Numerics;

namespace Age.Rendering.Services;

public class RenderingService : IDisposable
{
    private const bool DRAW_WIREFRAME = true;

    private readonly Shader                          diffuseShader;
    private readonly IndexBuffer                     indexBuffer;
    private readonly VulkanRenderer                  renderer;
    private readonly Dictionary<Texture, UniformSet> textureSets   = [];
    private readonly VertexBuffer                    vertexBuffer;
    private readonly IndexBuffer                     wireframeIndexBuffer;
    private readonly Shader                          wireframeShader;

    private RenderPass renderPass;

    private int  changes;
    private bool disposed;

    public RenderingService(VulkanRenderer renderer)
    {
        this.renderer = renderer;

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

        this.renderPass = renderer.Context.CreateRenderPass(colorPassCreateInfo);

        var vertices = new CanvasShader.Vertex[4]
        {
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        };

        this.vertexBuffer         = renderer.CreateVertexBuffer(vertices);
        this.indexBuffer          = renderer.CreateIndexBuffer([0u, 1, 2, 0, 2, 3]);
        this.wireframeIndexBuffer = renderer.CreateIndexBuffer([0u, 1, 1, 2, 2, 3, 3, 0, 0, 2]);
        this.diffuseShader        = renderer.CreateShaderAndWatch<CanvasShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);
        this.wireframeShader      = renderer.CreateShaderAndWatch<WireframeShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);

        this.diffuseShader.Changed += this.RequestDraw;
        this.wireframeShader.Changed += this.RequestDraw;

        this.renderer.Context.SwapchainRecreated += () =>
        {
            this.renderPass.Dispose();

            this.renderPass = renderer.Context.CreateRenderPass(colorPassCreateInfo);

            this.diffuseShader.RenderPass   = this.renderPass;
            this.wireframeShader.RenderPass = this.renderPass;

            this.RequestDraw();
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.renderer.DeferredDispose(this.renderPass);
                this.renderer.DeferredDispose(this.diffuseShader);
                this.renderer.DeferredDispose(this.wireframeShader);
                this.renderer.DeferredDispose(this.indexBuffer);
                this.renderer.DeferredDispose(this.vertexBuffer);
                this.renderer.DeferredDispose(this.wireframeIndexBuffer);
                this.renderer.DeferredDispose(this.textureSets.Values);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    private void Render(IWindow window, Element element, bool isWireframe)
    {
        UniformSet? lastUniformSet = default;

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

                        var constant = new CanvasShader.PushConstant
                        {
                            ViewportSize = windowSize.Cast<float>(),
                            Rect         = rectDrawCommand.Rect,
                            UV0          = rectDrawCommand.UV[0],
                            UV1          = rectDrawCommand.UV[1],
                            UV2          = rectDrawCommand.UV[2],
                            UV3          = rectDrawCommand.UV[3],
                            Color        = rectDrawCommand.Color,
                        };

                        if (uniformSet != lastUniformSet)
                        {
                            this.renderer.BindUniformSet(uniformSet);

                            lastUniformSet = uniformSet;
                        }

                        if (!isWireframe)
                        {
                            this.renderer.PushConstant(this.diffuseShader, constant);
                            this.renderer.DrawIndexed(this.indexBuffer);
                        }
                        else
                        {
                            this.renderer.PushConstant(this.wireframeShader, constant);
                            this.renderer.DrawIndexed(this.wireframeIndexBuffer);
                        }
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

    public Texture CreateTexture(Size<uint> size, ColorMode colorMode, TextureType textureType)
    {
        var textureCreate = new TextureCreate
        {
            Depth       = 1,
            Width       = size.Width,
            Height      = size.Height,
            TextureType = textureType,
            ColorMode   = colorMode,
        };

        return this.renderer.CreateTexture(textureCreate);
    }

    public Sampler CreateSampler() =>
        new() { Value = this.renderer.CreateSampler() };

    public void DestroySampler(Sampler sampler) =>
        this.renderer.DeferredDispose(sampler);

    public void Render(IWindow window)
    {
        this.renderer.BeginRenderPass(this.renderPass, window.Surface.Swapchain.Extent, window.Surface.CurrentBuffer);
        this.renderer.SetViewport(window.Surface);

        this.renderer.BindVertexBuffer(this.vertexBuffer);

        this.renderer.BindPipeline(this.diffuseShader);
        this.renderer.BindIndexBuffer(this.indexBuffer);

        foreach (var element in window.Content.Enumerate<Element>())
        {
            this.Render(window, element, false);
        }

        if (DRAW_WIREFRAME)
        {
            this.renderer.BindPipeline(this.wireframeShader);
            this.renderer.BindIndexBuffer(this.wireframeIndexBuffer);

            foreach (var element in window.Content.Enumerate<Element>())
            {
                this.Render(window, element, true);
            }
        }

        this.renderer.EndRenderPass();
    }

    public void FreeTexture(Texture texture)
    {
        this.renderer.DeferredDispose(texture);
        this.textureSets.Remove(texture);
    }

    public void RequestDraw()
    {
        // if (this.changes == 0)
        {
            this.changes++;
        }
    }

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

    public void UpdateTexture(Texture texture, uint[] data) =>
        this.renderer.UpdateTexture(texture, data);
}
