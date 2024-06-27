using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Vulkan;
using SkiaSharp;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

using Buffer          = Age.Rendering.Resources.Buffer;
using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;
using System.Runtime.InteropServices;
using Age.Rendering.Uniforms;

namespace Age.Rendering.RenderPasses;

public partial class GeometryRenderGraphPass : RenderGraphPass
{
    private readonly VkFormat           depthFormat;
    private readonly IndexBuffer        indexBuffer;
    private readonly RenderPass         renderPass;
    private readonly VkSampleCountFlags sampleCount;
    private readonly Sampler            sampler;
    private readonly Pipeline           pipeline;
    private readonly DateTime           startTime = DateTime.UtcNow;
    private readonly Texture            texture;
    private readonly Buffer[]           uniformBuffers;
    private readonly nint[]             uniformBuffersMapped = new nint[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly UniformSet[]       uniformSets;
    private readonly VertexBuffer       vertexBuffer;

    private Resources resources;

    public Image ColorImage  => this.resources.ResolveImage;
    public Image DepthImage  => this.resources.DepthImage;

    public override RenderPass RenderPass => this.renderPass;

    public unsafe GeometryRenderGraphPass(VulkanRenderer renderer, IWindow window) : base(renderer, window)
    {
        this.depthFormat    = renderer.DepthBufferFormat;
        this.sampleCount    = renderer.MaxUsableSampleCount;
        this.sampler        = renderer.CreateSampler();
        this.renderPass     = this.CreateRenderPass();
        this.uniformBuffers = this.CreateUniformBuffers();
        this.texture        = this.LoadTexture();

        this.LoadModel(out this.vertexBuffer, out this.indexBuffer);
        this.resources = this.CreateFrameBuffers();

        this.pipeline      = renderer.CreatePipelineAndWatch<GeometryShader, GeometryShader.Vertex, GeometryShader.PushConstant>(new() { RasterizationSamples = this.sampleCount, FrontFace = VkFrontFace.CounterClockwise }, this.renderPass);
        this.uniformSets = CreateUniformSets(this.pipeline, this.uniformBuffers, this.sampler, this.texture);
    }

    private static unsafe UniformSet[] CreateUniformSets(Pipeline pipeline, Buffer[] uniformBuffers, Sampler sampler, Texture texture)
    {
        var combinedImageSampler = new CombinedImageSamplerUniform
        {
            Binding     = 1,
            Sampler     = sampler,
            Texture     = texture,
            ImageLayout = VkImageLayout.ShaderReadOnlyOptimal
        };

        var uniformSets = new UniformSet[VulkanContext.MAX_FRAMES_IN_FLIGHT];

        for (var i = 0; i < uniformSets.Length; i++)
        {
            var uniformBuffer = new UniformBufferUniform
            {
                Binding = 0,
                Buffer  = uniformBuffers[i],
            };

            uniformSets[i] = new UniformSet(pipeline, [uniformBuffer, combinedImageSampler]);
        }

        return uniformSets;
    }

    private unsafe Resources CreateFrameBuffers()
    {
        var clientSize = this.Window.ClientSize;

        var colorImageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers = 1,
            Extent      = new()
            {
                Width  = clientSize.Width,
                Height = clientSize.Height,
                Depth  = 1,
            },
            Format        = this.Window.Surface.Swapchain.Format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = this.sampleCount,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.TransientAttachment | VkImageUsageFlags.ColorAttachment,
        };

        var colorImage = this.Renderer.CreateImage(colorImageCreateInfo);

        var resolveImageCreateInfo = colorImageCreateInfo;

        resolveImageCreateInfo.Usage = this.Window.Surface.Swapchain.ImageUsage | VkImageUsageFlags.Sampled;
        resolveImageCreateInfo.Samples = VkSampleCountFlags.N1;

        var resolveImage = this.Renderer.CreateImage(resolveImageCreateInfo);

        var depthImageCreateInfo = colorImageCreateInfo;

        depthImageCreateInfo.Usage   = VkImageUsageFlags.DepthStencilAttachment;
        depthImageCreateInfo.Format  = this.depthFormat;

        var depthImage = this.Renderer.CreateImage(depthImageCreateInfo);

        var framebufferCreateInfo = new FramebufferCreateInfo
        {
            RenderPass  = this.renderPass,
            Attachments =
            [
                new FramebufferCreateInfo.Attachment
                {
                    Image       = colorImage,
                    ImageAspect = VkImageAspectFlags.Color,
                },
                new FramebufferCreateInfo.Attachment
                {
                    Image       = resolveImage,
                    ImageAspect = VkImageAspectFlags.Color,
                },
                new FramebufferCreateInfo.Attachment
                {
                    Image       = depthImage,
                    ImageAspect = VkImageAspectFlags.Depth,
                },
            ]
        };

        var framebuffer = this.Renderer.CreateFramebuffer(framebufferCreateInfo);

        return new()
        {
            Framebuffer  = framebuffer,
            ColorImage   = colorImage,
            DepthImage   = depthImage,
            ResolveImage = resolveImage,
        };
    }

    private RenderPass CreateRenderPass()
    {
        var createInfo = new RenderPassCreateInfo
        {
            SubPasses =
            [
                new RenderPassCreateInfo.SubPass()
                {
                    PipelineBindPoint = VkPipelineBindPoint.Graphics,
                    ColorAttachments  =
                    [
                        new RenderPassCreateInfo.ColorAttachment()
                        {
                            Color = new VkAttachmentDescription()
                            {
                                Format         = this.Window.Surface.Swapchain.Format,
                                Samples        = this.sampleCount,
                                LoadOp         = VkAttachmentLoadOp.Clear,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare,
                                InitialLayout  = VkImageLayout.Undefined,
                                FinalLayout    = VkImageLayout.ColorAttachmentOptimal,
                            },
                            Resolve = new VkAttachmentDescription
                            {
                                Format         = this.Window.Surface.Swapchain.Format,
                                Samples        = VkSampleCountFlags.N1,
                                LoadOp         = VkAttachmentLoadOp.DontCare,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare,
                                InitialLayout  = VkImageLayout.Undefined,
                                FinalLayout    = VkImageLayout.ShaderReadOnlyOptimal,
                            }
                        }
                    ],
                    DepthStencilAttachment = new VkAttachmentDescription
                    {
                        Format         = this.depthFormat,
                        Samples        = this.sampleCount,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StoreOp        = VkAttachmentStoreOp.DontCare,
                        StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                        StencilStoreOp = VkAttachmentStoreOp.DontCare,
                        InitialLayout  = VkImageLayout.Undefined,
                        FinalLayout    = VkImageLayout.DepthStencilAttachmentOptimal,
                    }
                }
            ],
        };

        return this.Renderer.CreateRenderPass(createInfo);
    }

    private unsafe Buffer[] CreateUniformBuffers()
    {
        var size = (ulong)sizeof(UniformBufferObject);

        var uniformBuffers = new Buffer[VulkanContext.MAX_FRAMES_IN_FLIGHT];

        for (var i = 0; i < VulkanContext.MAX_FRAMES_IN_FLIGHT; i++)
        {
            uniformBuffers[i] = this.Renderer.CreateBuffer(size, VkBufferUsageFlags.UniformBuffer, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
            uniformBuffers[i].Allocation.Memory.Map(0, size, 0, out this.uniformBuffersMapped[i]);
        }

        return uniformBuffers;
    }

    private void LoadModel(out VertexBuffer vertexBuffer, out IndexBuffer indexBuffer)
    {
        var data = WavefrontLoader.Load(Path.Join(AppContext.BaseDirectory, "Assets", "Models", "viking_room.obj"));

        var uniqueVertices = new Dictionary<GeometryShader.Vertex, uint>();

        var indices  = new List<uint>();
        var vertices = new List<GeometryShader.Vertex>();

        foreach (var obj in data.Objects)
        {
            foreach (var item in obj.Mesh.Faces.SelectMany(x => x.Indices))
            {
                var pos      = data.Attributes.Vertices[item.Index];
                var color    = item.ColorIndex > -1 ? data.Attributes.Colors[item.ColorIndex] : new(1, 0, 0);
                var texCoord = data.Attributes.TexCoords[item.TexCoordIndex];

                texCoord.Y = 1 - texCoord.Y;

                var vertex = new GeometryShader.Vertex(pos, color, texCoord);

                if (!uniqueVertices.TryGetValue(vertex, out var index))
                {
                    uniqueVertices[vertex] = index = (uint)vertices.Count;
                    vertices.Add(vertex);
                }

                indices.Add(index);
            }
        }

        vertexBuffer = this.Renderer.CreateVertexBuffer(vertices.AsSpan());
        indexBuffer  = this.Renderer.CreateIndexBuffer(indices.AsSpan());
    }

    private unsafe Texture LoadTexture()
    {
        using var stream = File.OpenRead(Path.Join(AppContext.BaseDirectory, "Assets", "Textures", "viking_room.png"));

        var bitmap = SKBitmap.Decode(stream);

        var pixels = bitmap.Pixels.AsSpan().Cast<SKColor, byte>();

        var textureCreateInfo = new TextureCreateInfo
        {
            Format      = VkFormat.B8G8R8A8Unorm,
            Width       = (uint)bitmap.Width,
            Height      = (uint)bitmap.Height,
            Depth       = 1,
            ImageType   = VkImageType.N2D,
        };

        return this.Renderer.CreateTexture(textureCreateInfo, pixels);
    }

    private void UpdateUniformBuffer()
    {
        var extent = this.Window.Surface.Swapchain.Extent;

        var now = DateTime.UtcNow;

        var time = Math.Max(0, (float)(now - this.startTime).TotalMilliseconds / 1000);

        var ubo = new UniformBufferObject
        {
            Model = Matrix4x4<float>.Rotated(new(0, 0, 1), time * (float)(90 * Angle.RADIANS)),
            View  = Matrix4x4<float>.LookingAt(new(2), new(0), new(0, 0, 1)),
            Proj  = Matrix4x4<float>.PerspectiveFov((float)(45 * Angle.RADIANS), extent.Width / (float)extent.Height, 0.1f, 10)
        };

        ubo.Proj[1, 1] *= -1;

        Marshal.StructureToPtr(ubo, this.uniformBuffersMapped[this.Window.Surface.CurrentBuffer], true);
    }

    protected override void OnDispose()
    {
        this.vertexBuffer.Dispose();
        this.indexBuffer.Dispose();
        this.renderPass.Dispose();
        this.pipeline.Dispose();
        this.texture.Dispose();
        this.sampler.Dispose();
        this.resources.Dispose();

        for (var i = 0; i < VulkanContext.MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.uniformBuffers[i].Dispose();
            this.uniformSets[i].Dispose();
        }
    }

    public unsafe override void Execute()
    {
        this.UpdateUniformBuffer();

        var commandBuffer = this.Renderer.CurrentCommandBuffer;

        commandBuffer.SetViewport(this.Window.Surface.Swapchain.Extent);

        var color = Color.Black;

        var colorClearValue = new VkClearValue();
        colorClearValue.Color.Float32[0] = color.R;
        colorClearValue.Color.Float32[1] = color.G;
        colorClearValue.Color.Float32[2] = color.B;
        colorClearValue.Color.Float32[3] = color.A;

        var depthClearValue = new VkClearValue();
        depthClearValue.DepthStencil.Depth = 1;

        commandBuffer.BeginRenderPass(this.renderPass, this.resources.Framebuffer, [colorClearValue, default, depthClearValue]);
        commandBuffer.BindPipeline(this.pipeline);
        commandBuffer.BindUniformSet(this.uniformSets[this.Renderer.CurrentFrame]);
        commandBuffer.BindVertexBuffer([this.vertexBuffer]);
        commandBuffer.BindIndexBuffer(this.indexBuffer);
        commandBuffer.DrawIndexed(this.indexBuffer);
        commandBuffer.EndRenderPass();
    }

    public override void Recreate()
    {
        this.resources.Dispose();
        this.resources = this.CreateFrameBuffers();
    }
}
