using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Sampler : Resource<VkSampler>
{
    internal Sampler(VkSampler sampler) : base(sampler)
    { }

    protected override void OnDispose() =>
        this.Value.Dispose();
}
