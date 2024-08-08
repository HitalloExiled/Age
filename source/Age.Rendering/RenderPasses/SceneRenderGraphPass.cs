using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Scene.Resources;
using Age.Rendering.Scene;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using System.Runtime.InteropServices;
using Age.Rendering.Extensions;

namespace Age.Rendering.RenderPasses;

public partial class SceneRenderGraphPass : RenderGraphPass
{
    private readonly VkFormat           depthFormat;
    private readonly FrameResource[]    frameResources = new FrameResource[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly RenderPass         renderPass;
    private readonly VkSampleCountFlags sampleCount;
    private readonly DateTime           startTime = DateTime.UtcNow;

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
                                InitialLayout  = VkImageLayout.ShaderReadOnlyOptimal,
                                FinalLayout    = VkImageLayout.ShaderReadOnlyOptimal,
                            },
                            Resolve = new VkAttachmentDescription
                            {
                                Format         = this.Window.Surface.Swapchain.Format,
                                Samples        = VkSampleCountFlags.N1,
                                LoadOp         = VkAttachmentLoadOp.DontCare,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare,
                                InitialLayout  = VkImageLayout.ShaderReadOnlyOptimal,
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

    private unsafe UboHandle UpdateUbo(Camera3D camera, Mesh mesh, in Matrix4x4<float> transform, in VkExtent2D viewport)
    {
        ref var frameResource = ref this.frameResources[this.Renderer.CurrentFrame];

        var size = (uint)sizeof(UniformBufferObject);

        var hashcode = camera.GetHashCode() ^ mesh.GetHashCode();

        if (!frameResource.Ubo.TryGetValue(hashcode, out var cameraBuffer))
        {
            var buffer = this.Renderer.CreateBuffer(size, VkBufferUsageFlags.UniformBuffer, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

            buffer.Allocation.Memory.Map(0, size, 0, out var handle);

            frameResource.Ubo[hashcode] = cameraBuffer = new(handle, buffer);
        }

        var ubo = new UniformBufferObject
        {
            Model = transform,
            View  = camera.TransformCache.Inverse(),
            Proj  = Matrix4x4<float>.PerspectiveFov(camera.FoV, viewport.Width / (float)viewport.Height, camera.Near, camera.Far)
        };

        ubo.Proj[1, 1] *= -1;

        Marshal.StructureToPtr(ubo, cameraBuffer.Handle, true);

        return cameraBuffer;
    }

    private unsafe UniformSet GetUniformSet(Camera3D camera, UboHandle cameraBuffer, Material material)
    {
        ref var frameResource = ref this.frameResources[this.Renderer.CurrentFrame];

        var hashcode = camera.GetHashCode() ^ material.GetHashCode();

        if (!frameResource.UniformSets.TryGetValue(hashcode, out var uniformSet))
        {
            var combinedImageSampler = new CombinedImageSamplerUniform
            {
                Binding     = 1,
                Sampler     = material.Diffuse.Sampler,
                Texture     = material.Diffuse,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
            };

            var uniformBuffer = new UniformBufferUniform
            {
                Binding = 0,
                Buffer  = cameraBuffer.Buffer,
            };

            frameResource.UniformSets[hashcode] = uniformSet = new UniformSet(material.Pipeline, [uniformBuffer, combinedImageSampler]);
        }

        return uniformSet;
    }

    protected unsafe override void OnDispose()
    {
        this.renderPass.Dispose();

        foreach (var resource in this.frameResources)
        {
            foreach (var ubo in resource.Ubo.Values)
            {
                this.Renderer.DeferredDispose(ubo.Buffer);

                NativeMemory.Free((void*)ubo.Handle);
            }

            this.Renderer.DeferredDispose(resource.UniformSets.Values);
        }
    }

    public unsafe override void Execute()
    {
        var commandBuffer = this.Renderer.CurrentCommandBuffer;

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
                foreach (var camera in scene.Cameras)
                {
                    foreach (var renderTarget in camera.RenderTargets)
                    {
                        commandBuffer.BeginRenderPass(this.RenderPass, renderTarget.Framebuffer, [colorClearValue, default, depthClearValue]);
                        commandBuffer.SetViewport(renderTarget.Size.ToExtent2D());

                        foreach (var entry in this.Window.Tree.Enumerate3DCommands())
                        {
                            switch (entry.Command)
                            {
                                case MeshCommand meshCommand:
                                    var ubo = this.UpdateUbo(camera, meshCommand.Mesh, entry.Transform, renderTarget.Size.ToExtent2D());

                                    commandBuffer.BindPipeline(meshCommand.Mesh.Material.Pipeline);
                                    commandBuffer.BindUniformSet(this.GetUniformSet(camera, ubo, meshCommand.Mesh.Material));
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
    }

    public override void Recreate()
    { }
}

