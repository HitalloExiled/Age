namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkApplicationInfo.html">VkApplicationInfo</see>
/// </summary>
public unsafe struct VkApplicationInfo
{
    public readonly VkStructureType sType;

    public void* pNext;
    public byte* pApplicationName;
    public uint  applicationVersion;
    public byte* pEngineName;
    public uint  engineVersion;
    public uint  apiVersion;

    public VkApplicationInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO;
}
