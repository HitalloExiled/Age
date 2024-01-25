namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSpecializationMapEntry.html">VkSpecializationMapEntry</see>
/// </summary>
public struct VkSpecializationMapEntry
{
    public uint  constantID;
    public uint  offset;
    public ulong size;
}
