namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkApplicationInfo.html">VkApplicationInfo</see>
/// </summary>
public unsafe struct VkApplicationInfo
{
    public readonly VkStructureType sType;

    public void* PNext;
    public byte* PApplicationName;
    public uint  ApplicationVersion;
    public byte* PEngineName;
    public uint  EngineVersion;
    public uint  ApiVersion;

    public VkApplicationInfo() =>
        this.sType = VkStructureType.ApplicationInfo;
}
