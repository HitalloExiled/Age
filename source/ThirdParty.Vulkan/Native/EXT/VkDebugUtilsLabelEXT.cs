namespace ThirdParty.Vulkan.Native.EXT;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsLabelEXT.html">VkDebugUtilsLabelEXT</see>
/// </summary>
public unsafe struct VkDebugUtilsLabelEXT
{
    public readonly VkStructureType sType;

    public void*       pNext;
    public byte*       pLabelName;
    public fixed float color[4];

    public VkDebugUtilsLabelEXT() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_LABEL_EXT;
}
