using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkVertexInputBindingDescription.html">VkVertexInputBindingDescription</see>
/// </summary>
public struct VkVertexInputBindingDescription
{
    public uint              Binding;
    public uint              Stride;
    public VkVertexInputRate InputRate;
}
