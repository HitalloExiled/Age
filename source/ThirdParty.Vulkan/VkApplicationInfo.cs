using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkApplicationInfo.html">VkApplicationInfo</see>
/// </summary>
public unsafe struct VkApplicationInfo
{
    public readonly VkStructureType SType;

    public void* PNext;
    public byte* PApplicationName;
    public uint  ApplicationVersion;
    public byte* PEngineName;
    public uint  EngineVersion;
    public uint  ApiVersion;

    public VkApplicationInfo() =>
        this.SType = VkStructureType.ApplicationInfo;
}
