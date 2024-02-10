using ThirdParty.Vulkan;

namespace Age.Rendering;

public record Sampler
{
    public required VkSampler Handler { get; init; }
}
