using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Commands;
using Age.Graphs;
using Age.Internal;
using Age.Numerics;
using Age.Rendering.Extensions;
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

public class Scene3DColorPass : Scene3DPass
{
    public override string Name => nameof(Scene3DPass);

    protected override ClearValues ClearValues => ClearValues.Default;

    private UniformSet GetUniformSet(Camera3D camera, Shader shader, Material material, BufferHandlePair cameraBuffer)
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

    private unsafe BufferHandlePair UpdateUbo(Camera3D camera, Mesh mesh, in VkExtent2D viewport)
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

    protected override void OnDisposed(bool disposing)
    {
        base.OnDisposed(disposing);

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

        if (this.Viewport!.Camera3D != null)
        {
            foreach (var command in this.Viewport.RenderContext.Buffer3D.Commands)
            {
                switch (command)
                {
                    case MeshCommand meshCommand:
                        var mesh       = Unsafe.As<Mesh>(meshCommand.Owner);
                        var ubo        = this.UpdateUbo(this.Viewport.Camera3D, mesh, this.Viewport.Size.ToExtent2D());
                        var shader     = mesh.Material.GetShader(this.RenderTarget, new() { Subpass = this.Index });
                        var uniformSet = this.GetUniformSet(this.Viewport.Camera3D, shader, mesh.Material, ubo);

                        commandBuffer.BindShader(shader);
                        commandBuffer.BindVertexBuffer([meshCommand.VertexBuffer]);
                        commandBuffer.BindIndexBuffer(meshCommand.IndexBuffer);
                        commandBuffer.BindUniformSet(uniformSet);
                        commandBuffer.DrawIndexed(meshCommand.IndexBuffer);

                        break;
                }
            }
        }
    }
}
