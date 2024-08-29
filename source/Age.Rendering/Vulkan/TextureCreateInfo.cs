using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Vulkan;

public readonly struct TextureCreateInfo
{
    public required VkFormat    Format    { get; init; }
    public required uint        Depth     { get; init; }
    public required uint        Height    { get; init; }
    public required VkImageType ImageType { get; init; }
    public required uint        Width     { get; init; }
}
