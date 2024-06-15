using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Scene.Resources;
using Age.Rendering.Scene;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

using Buffer = Age.Rendering.Resources.Buffer;
using Age.Rendering.Uniforms;

namespace Age.Rendering.RenderPasses;

public partial class SceneRenderGraphPass : RenderGraphPass
{
    private readonly struct FrameResource
    {
        public Dictionary<int, UniformSet>          UniformSets   { get; } = [];
        public Dictionary<Camera3D, (nint, Buffer)> CameraBuffers { get; } = [];

        public FrameResource() { }
    }

    private readonly VkFormat           depthFormat;
    private readonly FrameResource[]    frameResources = new FrameResource[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly RenderPass         renderPass;
    private readonly VkSampleCountFlags sampleCount;

    public override RenderPass RenderPass => this.renderPass;

    public SceneRenderGraphPass(VulkanRenderer renderer, IWindow window) : base(renderer, window)
    {
        this.depthFormat = renderer.DepthBufferFormat;
        this.sampleCount = renderer.MaxUsableSampleCount;
        this.renderPass  = this.CreateRenderPass();

        for (var i = 0; i < VulkanContext.MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.frameResources[i] = new();
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

    private unsafe (nint Handle, Buffer Buffer) GetCameraBuffer(Camera3D camera)
    {
        ref var frameResource = ref this.frameResources[this.Renderer.CurrentFrame];

        var size = (ulong)sizeof(UniformBufferObject);

        if (!frameResource.CameraBuffers.TryGetValue(camera, out var cameraBuffer))
        {
            var buffer = this.Renderer.CreateBuffer(size, VkBufferUsageFlags.UniformBuffer, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

            buffer.Allocation.Memory.Map(0, size, 0, out var handle);

            frameResource.CameraBuffers[camera] = cameraBuffer = (handle, buffer);
        }

        return cameraBuffer;
    }

    private unsafe UniformSet GetUniformSet(Camera3D camera, Material material)
    {
        ref var frameResource = ref this.frameResources[this.Renderer.CurrentFrame];

        var hashcode = camera.GetHashCode() ^ material.GetHashCode();

        if (!frameResource.UniformSets.TryGetValue(hashcode, out var uniformSet))
        {
            var cameraBuffer = this.GetCameraBuffer(camera);

            var combinedImageSampler = new CombinedImageSamplerUniform
            {
                Binding = 1,
                Sampler = material.Diffuse.Sampler,
                Texture = material.Diffuse,
            };

            var uniformBuffer = new UniformBufferUniform
            {
                Binding = 0,
                Buffer  = cameraBuffer.Buffer,
            };

            frameResource.UniformSets[hashcode] = uniformSet = this.Renderer.CreateUniformSet(material.Shader, [uniformBuffer, combinedImageSampler]);
        }

        return uniformSet;
    }

    protected override void OnDispose()
    { }

    public unsafe override void Execute()
    {
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

        foreach (var scene in this.Window.Tree.Scenes3D.AsSpan())
        {
            if (scene is Scene3D scene3D)
            {
                var camera = scene3D.Camera;

                if (camera?.RenderTarget != null)
                {
                    commandBuffer.BeginRenderPass(this.renderPass, camera.RenderTarget.Framebuffer, [colorClearValue, default, depthClearValue]);

                    var renderTarget = camera.RenderTarget;

                    foreach (var entry in this.Window.Tree.Enumerate3DCommands())
                    {
                        switch (entry.Command)
                        {
                            case MeshCommand meshCommand:
                                commandBuffer.BindPipeline(meshCommand.Material.Shader);
                                commandBuffer.BindUniformSet(this.GetUniformSet(camera, meshCommand.Material));
                                commandBuffer.BindVertexBuffer([meshCommand.VertexBuffer]);
                                commandBuffer.BindIndexBuffer(meshCommand.IndexBuffer);
                                commandBuffer.DrawIndexed(meshCommand.IndexBuffer);

                                break;
                            default:
                                break;
                        }
                    }

                    commandBuffer.EndRenderPass();
                }
            }
        }
    }

    public override void Recreate()
    { }
}
