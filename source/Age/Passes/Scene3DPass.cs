using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Age.Commands;
using Age.Core.Extensions;
using Age.Graphs;
using Age.Internal;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using Age.Resources;
using Age.Scenes;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Passes;

public abstract partial class Scene3DPass : RenderPass
{
    protected FrameResource[] FrameResources { get; } = new FrameResource[VulkanContext.MAX_FRAMES_IN_FLIGHT];

    [AllowNull]
    public override Texture2D Output => this.RenderGraph?.Viewport.Texture ?? Texture2D.Default;

    protected override ClearValues ClearValues => ClearValues.Default;

    protected abstract CommandFilter CommandFilter { get; }

    protected Scene3DPass() =>
        this.FrameResources.AsSpan().Fill(new());

    protected UniformSet GetUniformSet(Shader shader, Material material, Camera3D camera, BufferHandlePair cameraBuffer)
    {
        ref var frameResource = ref this.FrameResources[VulkanRenderer.Singleton.CurrentFrame];

        var key = (camera, material);

        if (!frameResource.UniformSets.TryGetValue(key, out var uniformSet))
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

            frameResource.UniformSets[key] = uniformSet = new UniformSet(shader, [uniformBuffer, combinedImageSampler]);
        }

        return uniformSet;
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            foreach (var resource in this.FrameResources)
            {
                foreach (var ubo in resource.Ubo.Values)
                {
                    VulkanRenderer.Singleton.DeferredDispose(ubo.Buffer);
                }

                foreach (var uniformSet in resource.UniformSets.Values)
                {
                    VulkanRenderer.Singleton.DeferredDispose(uniformSet);
                }
            }
        }
    }

    protected override void Record(RenderContext context)
    {
        var commandBuffer = this.CommandBuffer;

        commandBuffer.SetStencilReference(VkStencilFaceFlags.FrontAndBack, 0);

        if (this.Viewport!.Camera3D is Camera3D camera)
        {
            foreach (var command in context.Buffer3D)
            {
                if (command.CommandFilter.HasAnyFlag(this.CommandFilter))
                {
                    switch (command)
                    {
                        case MeshCommand meshCommand:
                            this.Record(camera, meshCommand);

                            break;
                    }
                }
            }
        }
    }

    protected unsafe BufferHandlePair UpdateUbo(Camera3D camera, Mesh mesh, in VkExtent2D viewport)
    {
        ref var frameResource = ref this.FrameResources[VulkanRenderer.Singleton.CurrentFrame];

        var size = (uint)sizeof(UniformBufferObject);

        var key = (camera, mesh);

        if (!frameResource.Ubo.TryGetValue(key, out var cameraBuffer))
        {
            var buffer = new Buffer(size, VkBufferUsageFlags.UniformBuffer, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

            buffer.Allocation.Memory.Map(0, size, 0, out var handle);

            frameResource.Ubo[key] = cameraBuffer = new(handle, buffer);
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

    protected abstract void Record(Camera3D camera, MeshCommand command);
}
