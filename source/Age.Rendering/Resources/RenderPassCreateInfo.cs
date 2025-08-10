using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public readonly partial struct RenderPassCreateInfo()
{
    public readonly SubPass[]             SubPasses           { get; init; } = [];
    public readonly VkSubpassDependency[] SubpassDependencies { get; init; } = [];
}
