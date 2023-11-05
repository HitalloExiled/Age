using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;
using Age.Vulkan.Native.Types;

namespace Age.Rendering.Vulkan.Handlers;

public record TextureHandler
{
    public required Allocation            Allocation  { get; init; }
    public required uint                  Depth       { get; init; }
    public required VkFormat              Format      { get; init; }
    public required uint                  Height      { get; init; }
    public required VkImage               Image       { get; init; }
    public required VkImageTiling         ImageTiling { get; init; }
    public required VkImageType           ImageType   { get; init; }
    public required VkImageUsageFlags     ImageUsage  { get; init; }
    public required VkImageView           ImageView   { get; init; }
    public required VkSampleCountFlagBits SampleCount { get; init; }
    public required VkSampleCountFlagBits Samples     { get; init; }
    public required uint                  Width       { get; init; }
}
