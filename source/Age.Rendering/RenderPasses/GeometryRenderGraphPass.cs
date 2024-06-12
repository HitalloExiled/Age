using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Vulkan.Uniforms;
using Age.Rendering.Vulkan;
using SkiaSharp;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

using Buffer = Age.Rendering.Resources.Buffer;
using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;
using System.Runtime.InteropServices;

namespace Age.Rendering.RenderPasses;

public partial class GeometryRenderGraphPass : RenderGraphPass
{
    private readonly VkFormat     depthFormat;
    private readonly IndexBuffer  indexBuffer;
    private readonly RenderPass   renderPass;
    private readonly Sampler      sampler;
    private readonly Shader       shader;
    private readonly DateTime     startTime = DateTime.UtcNow;
    private readonly Texture      texture;
    private readonly Buffer[]     uniformBuffers;
    private readonly nint[]       uniformBuffersMapped = new nint[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly UniformSet[] uniformSets;
    private readonly VertexBuffer vertexBuffer;

    private Framebuffer[] framebuffers;
    private Image[]       colorImages;
    private Image[]       depthImages;

    public Image ColorImage => this.colorImages[this.Window.Surface.CurrentBuffer];
    public Image DepthImage => this.depthImages[this.Window.Surface.CurrentBuffer];
    public Texture Texture  => this.texture;

    public unsafe GeometryRenderGraphPass(VulkanRenderer renderer, IWindow window) : base(renderer, window)
    {
        this.depthFormat    = renderer.FindSupportedFormat([VkFormat.D32Sfloat, VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint], VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);
        this.sampler        = renderer.CreateSampler();
        this.renderPass     = this.CreateRenderPass();
        this.uniformBuffers = this.CreateUniformBuffers();
        this.texture        = this.LoadTexture();

        this.LoadModel(out this.vertexBuffer, out this.indexBuffer);
        this.CreateFrameBuffers(out this.colorImages, out this.depthImages, out this.framebuffers);

        this.shader     = renderer.CreateShader<GeometryShader, GeometryShader.Vertex, GeometryShader.PushConstant>(new(), this.renderPass);
        this.uniformSets = this.CreateUniformSets(this.shader, this.uniformBuffers, this.sampler, this.texture);
    }

    private unsafe UniformSet[] CreateUniformSets(Shader shader, Buffer[] uniformBuffers, Sampler sampler, Texture texture)
    {
        var combinedImageSampler = new CombinedImageSamplerUniform
        {
            Binding = 1,
            Sampler = sampler,
            Texture = texture,
        };

        var uniformSets = new UniformSet[VulkanContext.MAX_FRAMES_IN_FLIGHT];

        for (var i = 0; i < uniformSets.Length; i++)
        {
            var uniformBuffer = new UniformBufferUniform
            {
                Binding = 0,
                Buffer  = uniformBuffers[i],
            };

            uniformSets[i] = this.Renderer.CreateUniformSet(shader, [uniformBuffer, combinedImageSampler]);
        }

        return uniformSets;
    }

    private unsafe void CreateFrameBuffers(out Image[] colorImages, out Image[] depthImages, out Framebuffer[] framebuffers)
    {
        var length = this.Window.Surface.Swapchain.Images.Length;

        framebuffers = new Framebuffer[length];
        colorImages  = new Image[length];
        depthImages  = new Image[length];

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
            Samples       = VkSampleCountFlags.N1,
            Tiling        = VkImageTiling.Optimal,
            // Usage         = VkImageUsageFlags.TransientAttachment | VkImageUsageFlags.ColorAttachment,
            Usage         = VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferSrc /* For Debug */,
        };

        for (var i = 0; i < length; i++)
        {
            colorImages[i] = this.Renderer.CreateImage(colorImageCreateInfo);

            var depthImageCreateInfo = colorImageCreateInfo;

            // depthImageCreateInfo.Usage = VkImageUsageFlags.DepthStencilAttachment;
            depthImageCreateInfo.Usage = VkImageUsageFlags.DepthStencilAttachment | VkImageUsageFlags.TransferSrc /* For Debug */;
            depthImageCreateInfo.Format = this.depthFormat;

            depthImages[i] = this.Renderer.CreateImage(depthImageCreateInfo);

            var framebufferCreateInfo = new FramebufferCreateInfo
            {
                RenderPass  = this.renderPass,
                Attachments =
                [
                    new FramebufferCreateInfo.Attachment
                    {
                        Image       = colorImages[i],
                        Format      = this.Window.Surface.Swapchain.Format,
                        ImageAspect = VkImageAspectFlags.Color,
                    },
                    new FramebufferCreateInfo.Attachment
                    {
                        Image       = depthImages[i],
                        Format      = this.depthFormat,
                        ImageAspect = VkImageAspectFlags.Depth,
                    },
                ]
            };

            this.framebuffers[i] = this.Renderer.CreateFramebuffer(framebufferCreateInfo);
        }
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
                                Samples        = VkSampleCountFlags.N1,
                                LoadOp         = VkAttachmentLoadOp.Clear,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare,
                                InitialLayout  = VkImageLayout.Undefined,
                                // FinalLayout    = VkImageLayout.ColorAttachmentOptimal,
                                FinalLayout    = VkImageLayout.TransferSrcOptimal /* For Debug */,
                            },
                            // Resolve = new VkAttachmentDescription
                            // {
                            //     Format         = this.Window.Surface.Swapchain.Format,
                            //     Samples        = VkSampleCountFlags.N1,
                            //     LoadOp         = VkAttachmentLoadOp.DontCare,
                            //     StoreOp        = VkAttachmentStoreOp.Store,
                            //     StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                            //     StencilStoreOp = VkAttachmentStoreOp.DontCare,
                            //     InitialLayout  = VkImageLayout.Undefined,
                            //     FinalLayout    = VkImageLayout.PresentSrcKHR
                            // }
                        }
                    ],
                    DepthStencilAttachment = new VkAttachmentDescription
                    {
                        Format         = this.depthFormat,
                        Samples        = VkSampleCountFlags.N1,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StoreOp        = VkAttachmentStoreOp.DontCare,
                        StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                        StencilStoreOp = VkAttachmentStoreOp.DontCare,
                        InitialLayout  = VkImageLayout.Undefined,
                        // FinalLayout    = VkImageLayout.DepthStencilAttachmentOptimal
                        FinalLayout    = VkImageLayout.TransferSrcOptimal /* For Debug */,
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

    private void DisposeFrameBuffers()
    {
        for (var i = 0; i < this.framebuffers.Length; i++)
        {
            this.colorImages[i].Dispose();
            this.depthImages[i].Dispose();
            this.framebuffers[i].Dispose();
        }
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
        indexBuffer  = this.Renderer.CreateIndexBuffer(indices);
    }

    private unsafe Texture LoadTexture()
    {
        using var stream = File.OpenRead(Path.Join(AppContext.BaseDirectory, "Assets", "Textures", "viking_room.png"));

        var bitmap = SKBitmap.Decode(stream);

        var pixels = bitmap.Pixels.AsSpan().Cast<SKColor, byte>();

        var textureCreateInfo = new TextureCreateInfo
        {
            ColorMode   = ColorMode.RGBA,
            Width       = (uint)bitmap.Width,
            Height      = (uint)bitmap.Height,
            Depth       = 1,
            TextureType = TextureType.N2D,
        };

        var texture = this.Renderer.CreateTexture(textureCreateInfo, pixels);

        return texture;
    }

    private void UpdateUniformBuffer()
    {
        var extent = this.Window.Surface.Swapchain.Extent;

        var now = DateTime.UtcNow;

        var time = Math.Max(0, (float)(now - this.startTime).TotalMilliseconds / 1000);

        var ubo = new UniformBufferObject
        {
            Model = Matrix4x4<float>.Rotate(new(0, 0, 1), time * (float)(90 * Angle.RADIANS)),
            View  = Matrix4x4<float>.LookAt(new(2), new(0), new(0, 0, 1)),
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
        this.shader.Dispose();
        this.texture.Dispose();
        this.sampler.Dispose();

        for (var i = 0; i < Math.Max(this.framebuffers.Length, VulkanContext.MAX_FRAMES_IN_FLIGHT); i++)
        {
            if (i < this.framebuffers.Length)
            {
                this.colorImages[i].Dispose();
                this.depthImages[i].Dispose();
                this.framebuffers[i].Dispose();
            }

            if (i < VulkanContext.MAX_FRAMES_IN_FLIGHT)
            {
                this.uniformBuffers[i].Dispose();
                this.uniformSets[i].Dispose();
             }
        }
    }

    public override void Execute()
    {
        this.UpdateUniformBuffer();

        var commandBuffer = this.Renderer.CurrentCommandBuffer;
        var framebuffer   = this.framebuffers[this.Window.Surface.CurrentBuffer];

        commandBuffer.SetViewport(this.Window.Surface.Swapchain.Extent);
        commandBuffer.BeginRenderPass(this.renderPass, framebuffer, Color.Black);
        commandBuffer.BindPipeline(this.shader);
        commandBuffer.BindUniformSet(this.uniformSets[this.Renderer.CurrentFrame]);
        commandBuffer.BindVertexBuffer([this.vertexBuffer]);
        commandBuffer.BindIndexBuffer(this.indexBuffer);
        commandBuffer.DrawIndexed(this.indexBuffer);

        commandBuffer.EndRenderPass();
    }

    public override void Recreate()
    {
        this.DisposeFrameBuffers();
        this.CreateFrameBuffers(out this.colorImages, out this.depthImages, out this.framebuffers);
    }
}
