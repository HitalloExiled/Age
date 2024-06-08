using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Storage;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;
using Buffer          = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.RenderPasses;

public partial class GeometryRenderGraphPass : RenderGraphPass
{

    private readonly Framebuffer[] framebuffers = new Framebuffer[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly Image[]       images = new Image[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly IndexBuffer   indexBuffer;
    private readonly RenderPass    renderPass;
    private readonly Sampler       sampler;
    private readonly Shader        shader;
    private readonly Texture       texture;
    private readonly Buffer[]      uniformBuffers = new Buffer[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly nint[]        uniformBuffersMapped = new nint[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly UniformSet    uniformSet;
    private readonly VertexBuffer  vertexBuffer;

    public unsafe GeometryRenderGraphPass(VulkanRenderer renderer, IWindow window, TextureStorage textureStorage) : base(renderer, window)
    {
        this.texture = this.LoadTexture();
        this.LoadModel(out this.vertexBuffer, out this.indexBuffer);

        this.renderPass = this.CreateRenderPass();
        this.shader = renderer.CreateShader<GeometryShader, GeometryShader.Vertex, GeometryShader.PushConstant>(new(), this.renderPass);
        this.sampler = textureStorage.DefaultSampler;
        this.uniformSet = textureStorage.GetUniformSet(this.shader, this.texture, this.sampler);

        this.CreateFrameBuffers();
    }

    private unsafe void CreateFrameBuffers()
    {
        for (var i = 0; i < VulkanContext.MAX_FRAMES_IN_FLIGHT; i++)
        {
            var size = (ulong)sizeof(UniformBufferObject);

            this.uniformBuffers[i] = this.Renderer.CreateBuffer(size, VkBufferUsageFlags.UniformBuffer, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
            this.uniformBuffers[i].Allocation.Memory.Map(0, size, 0, out this.uniformBuffersMapped[i]);

            var createInfo = new FramebufferCreateInfo
            {
                RenderPass = this.renderPass,
                Attachments =
                [
                    new FramebufferCreateInfo.Attachment
                    {
                        Image       = this.images[i],
                        Format      = this.Window.Surface.Swapchain.Format,
                        ImageAspect = VkImageAspectFlags.Color,
                    },
                ]
            };

            this.framebuffers[i] = this.Renderer.CreateFramebuffer(createInfo);
        }
    }

    private unsafe Texture LoadTexture()
    {
        var textureCreate = new TextureCreate
        {
            ColorMode = ColorMode.RGBA,
            Width       = 1024,
            Height      = 768,
            Depth       = 1,
            TextureType = TextureType.N2D,
        };

        return this.Renderer.CreateTexture(textureCreate);
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
                                FinalLayout    = VkImageLayout.ColorAttachmentOptimal
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
                                FinalLayout    = VkImageLayout.PresentSrcKHR
                            }
                        }
                    ],
                    DepthStencilAttachment = new VkAttachmentDescription
                    {
                        Format         = this.Renderer.FindSupportedFormat([VkFormat.D32Sfloat, VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint], VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment),
                        Samples        = VkSampleCountFlags.N1,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StoreOp        = VkAttachmentStoreOp.DontCare,
                        StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                        StencilStoreOp = VkAttachmentStoreOp.DontCare,
                        InitialLayout  = VkImageLayout.Undefined,
                        FinalLayout    = VkImageLayout.DepthStencilAttachmentOptimal
                    }
                }
            ],
        };

        return this.Renderer.CreateRenderPass(createInfo);
    }

    private void LoadModel(out VertexBuffer vertexBuffer, out IndexBuffer indexBuffer)
    {
        var data = WavefrontLoader.Load(Path.Join(AppContext.BaseDirectory, "Models", "viking_room.obj"));

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

    public override void Execute()
    {
        var commandBuffer = this.Renderer.CurrentCommandBuffer;
        var framebuffer   = this.framebuffers[this.Window.Surface.CurrentBuffer];

        commandBuffer.BeginRenderPass(this.renderPass, framebuffer, Color.Black);
        commandBuffer.BindPipeline(this.shader);
        commandBuffer.BindUniformSet(this.uniformSet);
        commandBuffer.BindVertexBuffer([this.vertexBuffer]);
        commandBuffer.BindIndexBuffer(this.indexBuffer);
        commandBuffer.DrawIndexed(this.indexBuffer);

        commandBuffer.EndRenderPass();
    }

    public override void Recreate()
    { }

    protected override void OnDispose()
    {
        this.vertexBuffer.Dispose();
        this.indexBuffer.Dispose();
        this.renderPass.Dispose();
        this.shader.Dispose();
        this.texture.Dispose();

        for (var i = 0; i < this.uniformBuffers.Length; i++)
        {
            this.uniformBuffers[i].Dispose();
        }
    }
}
