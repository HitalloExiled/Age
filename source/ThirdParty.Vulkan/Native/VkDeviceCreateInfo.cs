namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDeviceCreateInfo.html">VkDeviceCreateInfo</see>
/// </summary>
public unsafe struct VkDeviceCreateInfo
{
    public readonly VkStructureType  SType;
    public void*                     PNext;
    public VkDeviceCreateFlags       Flags;
    public uint                      QueueCreateInfoCount;
    public VkDeviceQueueCreateInfo*  PQueueCreateInfos;

    [Obsolete("Ignored")]
    public uint EnabledLayerCount;

    [Obsolete("Ignored")]
    public byte** PpEnabledLayerNames;

    public uint                      EnabledExtensionCount;
    public byte**                    PpEnabledExtensionNames;
    public VkPhysicalDeviceFeatures* PEnabledFeatures;

    public VkDeviceCreateInfo() =>
        this.SType = VkStructureType.DeviceCreateInfo;
}
