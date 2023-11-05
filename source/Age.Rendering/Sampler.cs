using Age.Vulkan.Native.Types;

namespace Age.Rendering;

public record Sampler
{
    public required VkSampler Handler { get; init; }
}
