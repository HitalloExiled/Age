using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Sampler : Resource<VkSampler>
{
    internal Sampler(VulkanRenderer renderer, VkSampler sampler) : base(renderer, sampler)
    { }

    protected override void OnDispose() =>
        this.Value.Dispose();
}
