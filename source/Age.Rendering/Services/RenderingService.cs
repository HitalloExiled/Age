#define DUMP_IMAGES

using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;
using Age.Rendering.Storage;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using System.Diagnostics.CodeAnalysis;
using SkiaSharp;

namespace Age.Rendering.Services;

internal partial class RenderingService : IRenderingService
{
    private const bool DRAW_WIREFRAME = true;

    private readonly Shader         diffuseShader;
    private readonly IndexBuffer    indexBuffer;
    private readonly Shader         objectIdShader;
    private readonly VulkanRenderer renderer;
    private readonly TextureStorage textureStorage;
    private readonly VertexBuffer   vertexBuffer;
    private readonly IndexBuffer    wireframeIndexBuffer;
    private readonly Shader         wireframeShader;

    private RenderPass canvasRenderPass;
    private int        changes;
    private bool       disposed;
    private Image      objectIdImage;
    private RenderPass objectIdRenderPass;

    public RenderingService(VulkanRenderer renderer, TextureStorage textureStorage)
    {
        this.renderer       = renderer;
        this.textureStorage = textureStorage;

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

        this.CreateRenderPasses();

        this.diffuseShader   = renderer.CreateShaderAndWatch<CanvasShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.canvasRenderPass);
        this.wireframeShader = renderer.CreateShaderAndWatch<CanvasWireframeShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.canvasRenderPass);
        this.objectIdShader  = renderer.CreateShaderAndWatch<CanvasObjectIdShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.objectIdRenderPass);

        this.diffuseShader.Changed += this.RequestDrawIncremental;
        this.wireframeShader.Changed += this.RequestDrawIncremental;
        this.objectIdShader.Changed += this.RequestDrawIncremental;

        this.renderer.Context.SwapchainRecreated += this.OnSwapchainRecreated;
    }

#if DUMP_IMAGES
    private static void SaveToFile(uint[] data, VkExtent3D extent)
    {
        static SKColor convert(uint value) => new(value);

        var pixels = data.Select(convert).ToArray();

        var bitmap = new SKBitmap((int)extent.Width, (int)extent.Height)
        {
            Pixels = pixels
        };

        var skimage = SKImage.FromBitmap(bitmap);

        try
        {
            using var stream = File.OpenWrite(Path.Join(Directory.GetCurrentDirectory(), $"ObjectId.png"));

            skimage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
        }
        catch
        {

        }
    }
#endif

    [MemberNotNull(nameof(objectIdImage), nameof(canvasRenderPass), nameof(objectIdRenderPass))]
    private void CreateRenderPasses()
    {
        var swapchain = Surface.Entries[0].Swapchain;

        var colorPassCreateInfo = new RenderPass.CreateInfo
        {
            FrameBufferCount = swapchain.Images.Length,
            Extent           = swapchain.Extent,
            SubPasses =
            [
                new()
                {
                    PipelineBindPoint = VkPipelineBindPoint.Graphics,
                    Format            = swapchain.Format,
                    Images            = swapchain.Images,
                    ImageAspect       = VkImageAspectFlags.Color,
                    ColorAttachments =
                    [
                        new()
                        {
                            Layout = VkImageLayout.ColorAttachmentOptimal,
                            Color  = new VkAttachmentDescription
                            {
                                Format         = swapchain.Format,
                                Samples        = VkSampleCountFlags.N1,
                                InitialLayout  = VkImageLayout.Undefined,
                                FinalLayout    = VkImageLayout.PresentSrcKHR,
                                LoadOp         = VkAttachmentLoadOp.Clear,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare
                            },
                        }
                    ],
                }
            ]
        };

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = new() { Width = swapchain.Extent.Width, Height = swapchain.Extent.Height, Depth = 1 },
            Format        = swapchain.Format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = VkSampleCountFlags.N1,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.Sampled | VkImageUsageFlags.ColorAttachment,
        };

        this.objectIdImage = this.renderer.CreateImage(imageCreateInfo);

        var objectIdPassCreateInfo = new RenderPass.CreateInfo
        {
            FrameBufferCount = 1,
            Extent           = swapchain.Extent,
            SubPasses        =
            [
                new()
                {
                    PipelineBindPoint = VkPipelineBindPoint.Graphics,
                    Images            = [this.objectIdImage.Value],
                    ImageAspect       = VkImageAspectFlags.Color,
                    Format            = swapchain.Format,
                    ColorAttachments  =
                    [
                        new()
                        {
                            Layout = VkImageLayout.ColorAttachmentOptimal,
                            Color  = new VkAttachmentDescription
                            {
                                Format         = swapchain.Format,
                                Samples        = VkSampleCountFlags.N1,
                                InitialLayout  = VkImageLayout.Undefined,
                                FinalLayout    = VkImageLayout.TransferSrcOptimal,
                                LoadOp         = VkAttachmentLoadOp.Clear,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare
                            },
                        }
                    ],
                }
            ]
        };

        this.canvasRenderPass   = this.renderer.CreateRenderPass(colorPassCreateInfo);
        this.objectIdRenderPass = this.renderer.CreateRenderPass(objectIdPassCreateInfo);
    }

    private void OnSwapchainRecreated()
    {
        this.canvasRenderPass.Dispose();
        this.objectIdRenderPass.Dispose();
        this.objectIdImage.Dispose();

        this.CreateRenderPasses();

        this.diffuseShader.RenderPass = this.canvasRenderPass;
        this.wireframeShader.RenderPass = this.canvasRenderPass;
        this.objectIdShader.RenderPass = this.objectIdRenderPass;

        this.RequestDrawIncremental();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.renderer.DeferredDispose(this.objectIdImage);
                this.renderer.DeferredDispose(this.canvasRenderPass);
                this.renderer.DeferredDispose(this.objectIdRenderPass);
                this.renderer.DeferredDispose(this.diffuseShader);
                this.renderer.DeferredDispose(this.wireframeShader);
                this.renderer.DeferredDispose(this.objectIdShader);
                this.renderer.DeferredDispose(this.indexBuffer);
                this.renderer.DeferredDispose(this.vertexBuffer);
                this.renderer.DeferredDispose(this.wireframeIndexBuffer);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    private void RenderCanvas(IWindow window, Node2D node, RenderFlags flags)
    {
        UniformSet? lastUniformSet = default;

        var windowSize = window.ClientSize.Cast<float>();

        var transform = (Matrix3x2<float>)node.TransformCache;

        foreach (var command in node.Commands)
        {
            switch (command)
            {
                case RectDrawCommand rectDrawCommand:
                    {
                        var constant = new CanvasShader.PushConstant
                        {
                            Border    = rectDrawCommand.Border,
                            Color     = rectDrawCommand.Color,
                            Flags     = rectDrawCommand.Flags,
                            Rect      = rectDrawCommand.Rect,
                            Transform = transform,
                            UV        = rectDrawCommand.SampledTexture.UV,
                            Viewport  = windowSize,
                        };

                        if (!flags.HasFlag(RenderFlags.Wireframe))
                        {
                            var uniformSet = this.textureStorage.GetUniformSet(this.diffuseShader, rectDrawCommand.SampledTexture.Texture, rectDrawCommand.SampledTexture.Sampler);

                            if (uniformSet != null && uniformSet != lastUniformSet)
                            {
                                this.renderer.BindUniformSet(uniformSet);

                                lastUniformSet = uniformSet;
                            }

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

    private void Render(IWindow window, Span<RenderContext> contexts)
    {
        foreach (var context in contexts)
        {
            this.renderer.SetViewport(context.CommandBuffer, window.Surface);
            this.renderer.BindIndexBuffer(context.CommandBuffer, context.IndexBuffer);
            this.renderer.BindVertexBuffers(context.CommandBuffer, context.VertexBuffer);
            this.renderer.BindPipeline(context.CommandBuffer, context.Shader);
            this.renderer.BeginRenderPass(context.CommandBuffer, context.RenderPass, 0, Color.White);
        }

        var windowSize = window.ClientSize.Cast<float>();

        UniformSet? lastUniformSet = null;

        foreach (var node in window.Tree.Traverse<Node2D>(true))
        {
            var transform = (Matrix3x2<float>)node.TransformCache;

            foreach (var command in node.Commands)
            {
                switch (command)
                {
                    case RectDrawCommand rectDrawCommand:
                        {
                            var constant = new CanvasShader.PushConstant
                            {
                                Border    = rectDrawCommand.Border,
                                Flags     = rectDrawCommand.Flags,
                                Rect      = rectDrawCommand.Rect,
                                Transform = transform,
                                UV        = rectDrawCommand.SampledTexture.UV,
                                Viewport  = windowSize,
                            };

                            foreach (var context in contexts)
                            {
                                if (context.Shader == this.diffuseShader)
                                {
                                    var uniformSet = this.textureStorage.GetUniformSet(this.diffuseShader, rectDrawCommand.SampledTexture.Texture, rectDrawCommand.SampledTexture.Sampler);

                                    if (uniformSet != null && uniformSet != lastUniformSet)
                                    {
                                        this.renderer.BindUniformSet(uniformSet);

                                        lastUniformSet = uniformSet;
                                    }

                                    constant.Color = rectDrawCommand.Color;
                                }
                                else
                                {
                                    constant.Color = node.ObjectId | 0b_11111111_00000000_00000000_00000000;
                                }

                                this.renderer.PushConstant(context.CommandBuffer, context.Shader, constant);
                                this.renderer.DrawIndexed(context.CommandBuffer, context.IndexBuffer);
                            }

                            break;
                        }
                    default:
                        throw new Exception();
                }
            }
        }

        foreach (var context in contexts)
        {
            this.renderer.EndRenderPass(context.CommandBuffer);
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

    public unsafe uint[] GetObjectIdBuffer(IWindow window)
    {
        var image = this.objectIdImage;

        var size = image.Extent.Width * image.Extent.Height * sizeof(uint);

        using var buffer = this.renderer.CreateBuffer(size, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

        this.renderer.CopyImageToBuffer(image, buffer, image.Extent);

        buffer.Allocation.Memory.Map(0, size, 0, out var data);

        var pixels = new Span<uint>((uint*)data, (int)(size / 4)).ToArray();

#if DUMP_IMAGES
        SaveToFile(pixels, image.Extent);
#endif

        return pixels;
    }

    public void RequestDraw()
    {
        if (this.changes == 0)
        {
            this.changes++;
        }
    }

    public void Render(IWindow window)
    {
        this.renderer.SetViewport(window.Surface);
        this.renderer.BindIndexBuffer(this.indexBuffer);
        this.renderer.BindVertexBuffers(this.vertexBuffer);

        this.renderer.BeginRenderPass(this.canvasRenderPass, window.Surface.CurrentBuffer, Color.White);
        this.renderer.BindPipeline(this.diffuseShader);

        foreach (var node in window.Tree.Traverse<Node2D>(true))
        {
            this.RenderCanvas(window, node, default);
        }

#pragma warning disable IDE0035, CS0162
        if (DRAW_WIREFRAME)
        {
            this.renderer.BindPipeline(this.wireframeShader);
            this.renderer.BindIndexBuffer(this.wireframeIndexBuffer);

            foreach (var node in window.Tree.Traverse<Node2D>(true))
            {
                this.RenderCanvas(window, node, RenderFlags.Wireframe);
            }
        }
#pragma warning restore IDE0035, CS0162

        this.renderer.EndRenderPass();
    }

    public void Render(IEnumerable<IWindow> windows)
    {
        if (this.changes > 0)
        {
            var commandBuffer = this.renderer.Context.BeginSingleTimeCommands();

            Span<RenderContext> contexts =
            [
                new RenderContext
                {
                    CommandBuffer = commandBuffer,
                    Flags         = default,
                    IndexBuffer   = this.indexBuffer,
                    RenderPass    = this.objectIdRenderPass,
                    Shader        = this.objectIdShader,
                    VertexBuffer  = this.vertexBuffer,
                    Next          = [],
                },
                new RenderContext
                {
                    CommandBuffer = this.renderer.Context.Frame.CommandBuffer,
                    Flags         = default,
                    IndexBuffer   = this.indexBuffer,
                    RenderPass    = this.canvasRenderPass,
                    Shader        = this.diffuseShader,
                    VertexBuffer  = this.vertexBuffer,
                },
            ];

    #pragma warning disable IDE0035, CS0162
            if (DRAW_WIREFRAME)
            {
                contexts[1].Next =
                [
                    new RenderContext
                    {
                        CommandBuffer = this.renderer.Context.Frame.CommandBuffer,
                        Flags         = RenderFlags.Wireframe,
                        IndexBuffer   = this.wireframeIndexBuffer,
                        RenderPass    = this.canvasRenderPass,
                        Shader        = this.wireframeShader,
                        VertexBuffer  = this.vertexBuffer,
                        Next          = [],
                    },
                ];
            }
#pragma warning restore IDE0035, CS0162

            this.renderer.BeginFrame();

            foreach (var window in windows)
            {
                if (window.Visible && !window.Minimized && !window.Closed)
                {
                    this.Render(window, contexts);
                }
            }

            this.renderer.EndFrame();

            this.renderer.Context.EndSingleTimeCommands(commandBuffer);

            this.changes--;
        }
    }

    public void UpdateTexture(Texture texture, byte[] data) =>
        this.renderer.UpdateTexture(texture, data);
}
