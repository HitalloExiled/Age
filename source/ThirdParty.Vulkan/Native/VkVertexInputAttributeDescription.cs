namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkVertexInputAttributeDescription.html">VkVertexInputAttributeDescription</see>
/// </summary>
public struct VkVertexInputAttributeDescription
{
    public uint     location;
    public uint     binding;
    public VkFormat format;
    public uint     offset;
}
