namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSpecializationInfo.html">VkSpecializationInfo</see>
/// </summary>
public unsafe struct VkSpecializationInfo
{
    public uint                      mapEntryCount;
    public VkSpecializationMapEntry* pMapEntries;
    public ulong                     dataSize;
    public void*                     pData;
}
