using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Flags;
using Age.Numerics;
using Age.Rendering.Storage;
using System.Runtime.CompilerServices;

namespace Age.Rendering.Services;

internal class RenderingService : IDisposable
{
    private const bool DRAW_WIREFRAME = true;

    private readonly Shader         diffuseShader;
    private readonly IndexBuffer    indexBuffer;
    private readonly VulkanRenderer renderer;
    private readonly TextureStorage textureStorage;
    private readonly VertexBuffer   vertexBuffer;
    private readonly IndexBuffer    wireframeIndexBuffer;
    private readonly Shader         wireframeShader;

    private RenderPass renderPass;

    private int  changes;
    private bool disposed;

    public RenderingService(VulkanRenderer renderer, TextureStorage textureStorage)
    {
        this.renderer       = renderer;
        this.textureStorage = textureStorage;

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

        this.diffuseShader.Changed += this.RequestDrawIncremental;
        this.wireframeShader.Changed += this.RequestDrawIncremental;

        this.renderer.Context.SwapchainRecreated += () =>
        {
            this.renderPass.Dispose();

            this.renderPass = renderer.Context.CreateRenderPass(colorPassCreateInfo);

            this.diffuseShader.RenderPass   = this.renderPass;
            this.wireframeShader.RenderPass = this.renderPass;

            this.RequestDrawIncremental();
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
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    private void Render(IWindow window, Node2D node, bool isWireframe)
    {
        UniformSet? lastUniformSet = default;

        var windowSize = window.ClientSize.Cast<float>();

        var position = node.Transform.Position;

        ref var nodePosition = ref Unsafe.As<Vector2<float>, Point<float>>(ref position);

        foreach (var command in node.Commands)
        {
            switch (command)
            {
                case RectDrawCommand rectDrawCommand:
                    {
                        var uniformSet = this.textureStorage.GetUniformSet(this.diffuseShader, rectDrawCommand.SampledTexture.Texture, rectDrawCommand.SampledTexture.Sampler);

                        var rect = rectDrawCommand.Rect;
                        rect.Position += nodePosition;

                        var constant = new CanvasShader.PushConstant
                        {
                            ViewportSize = windowSize,
                            Rect         = rect,
                            UV           = rectDrawCommand.SampledTexture.UV,
                            Color        = rectDrawCommand.Color,
                        };

                        if (uniformSet != null && uniformSet != lastUniformSet)
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

    private void RequestDrawIncremental() =>
        this.changes++;

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
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

        foreach (var node in window.Tree.Traverse<Node2D>())
        {
            this.Render(window, node, false);
        }

        #pragma warning disable IDE0035, CS0162
        if (DRAW_WIREFRAME)
        {
            this.renderer.BindPipeline(this.wireframeShader);
            this.renderer.BindIndexBuffer(this.wireframeIndexBuffer);

            foreach (var node in window.Tree.Traverse<Node2D>(true))
            {
                this.Render(window, node, true);
            }
        }
        #pragma warning restore IDE0035, CS0162

        this.renderer.EndRenderPass();
    }

    public void RequestDraw()
    {
        if (this.changes == 0)
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
                    this.Render(window);
                }
            }

            this.renderer.EndFrame();

            this.changes--;
        }
    }

    public void UpdateTexture(Texture texture, byte[] data) =>
        this.renderer.UpdateTexture(texture, data);
}
