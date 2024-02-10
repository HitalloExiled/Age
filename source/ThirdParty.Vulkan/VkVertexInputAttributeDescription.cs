using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkVertexInputAttributeDescription.html">VkVertexInputAttributeDescription</see>
/// </summary>
public struct VkVertexInputAttributeDescription
{
    public uint     Location;
    public uint     Binding;
    public VkFormat Format;
    public uint     Offset;
}
