using Age.Commands;
using Age.Internal;
using Age.Numerics;
using Age.Rendering.Extensions;
using Age.Rendering.Resources;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using Age.Resources;
using Age.Scenes;
using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using Buffer = Age.Rendering.Resources.Buffer;
using System.Runtime.CompilerServices;

namespace Age.RenderPasses;

public sealed partial class SceneRenderGraphPass : RenderGraphPass
{
    private readonly VkFormat           depthFormat;
    private readonly FrameResource[]    frameResources = new FrameResource[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly RenderPass         renderPass;
    private readonly VkSampleCountFlags sampleCount;

    public override RenderPass RenderPass => this.renderPass;

    public SceneRenderGraphPass(VulkanRenderer renderer, Window window) : base(renderer, window)
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
                                InitialLayout  = VkImageLayout.ShaderReadOnlyOptimal,
                                FinalLayout    = VkImageLayout.ShaderReadOnlyOptimal,
                            },
                            // Resolve = new VkAttachmentDescription
                            // {
                            //     Format         = this.Window.Surface.Swapchain.Format,
                            //     Samples        = VkSampleCountFlags.N1,
                            //     LoadOp         = VkAttachmentLoadOp.DontCare,
                            //     StoreOp        = VkAttachmentStoreOp.Store,
                            //     StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                            //     StencilStoreOp = VkAttachmentStoreOp.DontCare,
                            //     InitialLayout  = VkImageLayout.ShaderReadOnlyOptimal,
                            //     FinalLayout    = VkImageLayout.ShaderReadOnlyOptimal,
                            // }
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

        return new(createInfo);
    }

    private UniformSet GetUniformSet(RenderTarget renderTarget, Camera3D camera, BufferHandlePair cameraBuffer, Material material)
    {
        ref var frameResource = ref this.frameResources[this.Renderer.CurrentFrame];

        var hashcode = camera.GetHashCode() ^ material.GetHashCode();

        if (!frameResource.UniformSets.TryGetValue(hashcode, out var uniformSet))
        {
            var combinedImageSampler = new CombinedImageSamplerUniform
            {
                Binding     = 1,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                Image       = material.Diffuse.Image,
                ImageView   = material.Diffuse.ImageView,
                Sampler     = material.Diffuse.Sampler,
            };

            var uniformBuffer = new UniformBufferUniform
            {
                Binding = 0,
                Buffer  = cameraBuffer.Buffer,
            };

            frameResource.UniformSets[hashcode] = uniformSet = new UniformSet(material.GetShader(renderTarget), [uniformBuffer, combinedImageSampler]);
        }

        return uniformSet;
    }

    private unsafe BufferHandlePair UpdateUbo(Camera3D camera, Mesh mesh, in VkExtent2D viewport)
    {
        ref var frameResource = ref this.frameResources[this.Renderer.CurrentFrame];

        var size = (uint)sizeof(UniformBufferObject);

        var hashcode = camera.GetHashCode() ^ mesh.GetHashCode();

        if (!frameResource.Ubo.TryGetValue(hashcode, out var cameraBuffer))
        {
            var buffer = new Buffer(size, VkBufferUsageFlags.UniformBuffer, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

            buffer.Allocation.Memory.Map(0, size, 0, out var handle);

            frameResource.Ubo[hashcode] = cameraBuffer = new(handle, buffer);
        }

        var ubo = new UniformBufferObject
        {
            Model = mesh.Transform,
            View  = camera.CachedMatrix.Inverse(),
            Proj  = Matrix4x4<float>.PerspectiveFov(camera.FoV, viewport.Width / (float)viewport.Height, camera.Near, camera.Far)
        };

        ubo.Proj[1, 1] *= -1;

        Marshal.StructureToPtr(ubo, cameraBuffer.Handle, true);

        return cameraBuffer;
    }
    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.renderPass.Dispose();

            foreach (var resource in this.frameResources)
            {
                foreach (var ubo in resource.Ubo.Values)
                {
                    this.Renderer.DeferredDispose(ubo.Buffer);
                }

                this.Renderer.DeferredDispose(resource.UniformSets.Values);
            }
        }
    }

    public override void Execute()
    {
        var commandBuffer = this.Renderer.CurrentCommandBuffer;

        Span<ClearValue> clearValues =
        [
            new(Color.Black),
            new(1, 0)
        ];

        var viewports = this.Window.RenderTree.GetViewports();

        for (var i = viewports.Length - 1; i > -1; i--)
        {
            var viewport = viewports[i];

            commandBuffer.BeginRenderPass(viewport.RenderTarget, clearValues);
            commandBuffer.SetViewport(viewport.RenderTarget.Size);
            commandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, 0);

            if (viewport.Camera3D != null)
            {
                foreach (var command in viewport.RenderContext.Buffer3D.Commands)
                {
                    switch (command)
                    {
                        case MeshCommand meshCommand:
                        {
                            var mesh = Unsafe.As<Mesh>(meshCommand.Owner);

                            var ubo = this.UpdateUbo(viewport.Camera3D, mesh, viewport.RenderTarget.Size.ToExtent2D());

                            commandBuffer.BindShader(mesh.Material.GetShader(viewport.RenderTarget));
                            commandBuffer.BindUniformSet(this.GetUniformSet(viewport.RenderTarget, viewport.Camera3D, ubo, mesh.Material));
                            commandBuffer.BindVertexBuffer([meshCommand.VertexBuffer]);
                            commandBuffer.BindIndexBuffer(meshCommand.IndexBuffer);
                            commandBuffer.DrawIndexed(meshCommand.IndexBuffer);

                            break;
                        }
                    }
                }
            }

            commandBuffer.EndRenderPass();
        }
    }

    public override void Recreate()
    { }
}
