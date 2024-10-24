using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Vulkan;

public struct TextureCreateInfo
{
    public required VkFormat    Format    { get; set; }
    public required uint        Depth     { get; set; }
    public required uint        Height    { get; set; }
    public required VkImageType ImageType { get; set; }
    public required uint        Width     { get; set; }
}
