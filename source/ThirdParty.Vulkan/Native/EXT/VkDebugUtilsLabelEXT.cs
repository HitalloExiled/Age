namespace ThirdParty.Vulkan.Native.EXT;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsLabelEXT.html">VkDebugUtilsLabelEXT</see>
/// </summary>
public unsafe struct VkDebugUtilsLabelEXT
{
    public readonly VkStructureType SType;

    public void*       PNext;
    public byte*       PLabelName;
    public fixed float Color[4];

    public VkDebugUtilsLabelEXT() =>
        this.SType = VkStructureType.DebugUtilsLabelEXT;
}
