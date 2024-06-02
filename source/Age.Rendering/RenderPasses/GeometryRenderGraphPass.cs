using Age.Core.Extensions;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;

namespace Age.Rendering.RenderPasses;

public class GeometryRenderGraphPass : RenderGraphPass
{
    private readonly VertexBuffer vertexBuffer;
    private readonly IndexBuffer  indexBuffer;
    private readonly Shader       shader;
    private readonly RenderPass   renderPass;
    private readonly Image        texture;

    public GeometryRenderGraphPass(VulkanRenderer renderer, IWindow window) : base(renderer, window)
    {
        this.LoadModel(out this.vertexBuffer, out this.indexBuffer);

        (this.renderPass, this.texture) = this.CreateRenderPass();
        this.shader     = renderer.CreateShader<GeometryShader, GeometryShader.Vertex, GeometryShader.PushConstant>(new(), this.renderPass);
    }

    private (RenderPass, Image) CreateRenderPass()
    {
        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = new()
            {
                Width  = default,
                Height = default,
                Depth  = 1,
            },
            Format        = this.Window.Surface.Swapchain.Format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = VkSampleCountFlags.N1,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.Sampled | VkImageUsageFlags.ColorAttachment,
        };

        var image = this.Renderer.CreateImage(imageCreateInfo);

        this.Renderer.UpdateImage(image, []);

        var createInfo = new RenderPass.CreateInfo
        {
            Extent           = default,
            FrameBufferCount = 1,
            SubPasses =
            [
                new RenderPass.CreateInfo.SubPass()
                {
                    Format            = this.Window.Surface.Swapchain.Format,
                    ImageAspect       = VkImageAspectFlags.Color,
                    Images            = [image],
                    PipelineBindPoint = VkPipelineBindPoint.Graphics,
                    ColorAttachments  =
                    [
                        new RenderPass.CreateInfo.ColorAttachment()
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
                        Format         = this.Renderer.Context.FindSupportedFormat([VkFormat.D32Sfloat, VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint], VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment),
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

        return (this.Renderer.CreateRenderPass(createInfo), image);
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
        throw new NotImplementedException();
    }

    public override void Recreate()
    { }

    protected override void OnDispose()
    {
        this.vertexBuffer.Dispose();
        this.indexBuffer.Dispose();
        this.renderPass.Dispose();
        this.shader.Dispose();
    }
}
