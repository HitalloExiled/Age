using Age.Core;
using Age.Core.Extensions;
using Age.Core.Interop;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public class UniformSet : Resource
{
    public Shader shader { get; }

    public DescriptorPool    DescriptorPool { get; }
    public VkDescriptorSet[] DescriptorSets { get; }

    public unsafe UniformSet(Shader shader, Span<Uniform> uniforms)
    {
        var key = CreatePoolKey(shader, uniforms);

        this.shader         = shader;
        this.DescriptorPool = DescriptorPool.CreateDescriptorPool(key);

        var descriptorSetLayoutHandle = shader.DescriptorSetLayout.Handle;

        var descriptorSetAllocateInfo = new VkDescriptorSetAllocateInfo
        {
            DescriptorSetCount = 1,
            PSetLayouts        = &descriptorSetLayoutHandle,
        };

        this.DescriptorSets = this.DescriptorPool.Value.AllocateDescriptorSets(descriptorSetAllocateInfo);

        this.Update(uniforms);
    }

    private static unsafe DescriptorPoolKey CreatePoolKey(Shader shader, Span<Uniform> uniforms)
    {
        DescriptorPoolKey poolKey = default;

        foreach (var uniform in uniforms)
        {
            if (uniform.Binding > shader.UniformBindings.Length || shader.UniformBindings[uniform.Binding] != uniform.Type)
            {
                throw new InvalidOperationException($"The provided shader expects that binding {uniform.Binding} to be of type {shader.UniformBindings[uniform.Binding]}");
            }

            poolKey[uniform.Type]++;
        }

        return poolKey;
    }

    protected override void OnDispose() =>
        this.DescriptorPool.FreeDescriptorSets(this.DescriptorSets);

    public unsafe void Update(Span<Uniform> uniforms)
    {
        using var disposables = new Disposables();

        var writes = new List<VkWriteDescriptorSet>();

        foreach (var uniform in uniforms)
        {
            switch (uniform)
            {
                case CombinedImageSamplerUniform combinedImageSampler:
                {
                    var descriptorImageInfo = new VkDescriptorImageInfo
                    {
                        Sampler     = combinedImageSampler.Texture.Sampler.Value.Handle,
                        ImageView   = combinedImageSampler.Texture.ImageView.Handle,
                        ImageLayout = combinedImageSampler.ImageLayout,
                    };

                    var pImageInfo = new NativeArray<VkDescriptorImageInfo>([descriptorImageInfo]);

                    disposables.Add(pImageInfo);

                    var writeDescriptorSet = new VkWriteDescriptorSet
                    {
                        DescriptorCount = 1,
                        DescriptorType  = VkDescriptorType.CombinedImageSampler,
                        DstBinding      = uniform.Binding,
                        DstSet          = this.DescriptorSets[0].Handle,
                        PImageInfo      = pImageInfo.AsPointer(),
                    };

                    writes.Add(writeDescriptorSet);

                    break;
                }
                case UniformBufferUniform uniformBuffer:
                {
                    var descriptorBufferInfo = new VkDescriptorBufferInfo
                    {
                        Buffer = uniformBuffer.Buffer.Value.Handle,
                        Offset = uniformBuffer.Buffer.Allocation.Offset,
                        Range  = uniformBuffer.Buffer.Allocation.Size,
                    };

                    var pBufferInfo = new NativeArray<VkDescriptorBufferInfo>([descriptorBufferInfo]);

                    disposables.Add(pBufferInfo);

                    var writeDescriptorSet = new VkWriteDescriptorSet
                    {
                        DescriptorCount = 1,
                        DescriptorType  = VkDescriptorType.UniformBuffer,
                        DstBinding      = uniform.Binding,
                        DstSet          = this.DescriptorSets[0].Handle,
                        PBufferInfo     = pBufferInfo.AsPointer(),
                    };

                    writes.Add(writeDescriptorSet);

                    break;
                }
                default:
                    throw new Exception();
            }
        }

        VulkanRenderer.Singleton.UpdateDescriptorSets(writes.AsSpan(), []);
    }
}
