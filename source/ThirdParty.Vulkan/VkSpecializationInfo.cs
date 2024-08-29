namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSpecializationInfo.html">VkSpecializationInfo</see>
/// </summary>
public unsafe struct VkSpecializationInfo
{
    public uint                      MapEntryCount;
    public VkSpecializationMapEntry* PMapEntries;
    public ulong                     DataSize;
    public void*                     PData;
}
