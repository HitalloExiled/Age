using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan.Native.EXT;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsObjectNameInfoEXT.html">VkDebugUtilsObjectNameInfoEXT</see>
/// </summary>
public unsafe struct VkDebugUtilsObjectNameInfoEXT
{
    public readonly VkStructureType SType;

    public void*        PNext;
    public VkObjectType ObjectType;
    public long         ObjectHandle;
    public byte*        PObjectName;

    public VkDebugUtilsObjectNameInfoEXT() =>
        this.SType = VkStructureType.DebugUtilsObjectNameInfoEXT;
}
