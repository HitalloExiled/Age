using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan.Native.EXT;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsObjectNameInfoEXT.html">VkDebugUtilsObjectNameInfoEXT</see>
/// </summary>
public unsafe struct VkDebugUtilsObjectNameInfoEXT
{
    public readonly VkStructureType sType;

    public void*        pNext;
    public VkObjectType objectType;
    public long         objectHandle;
    public byte*        pObjectName;

    public VkDebugUtilsObjectNameInfoEXT() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_OBJECT_NAME_INFO_EXT;
}
