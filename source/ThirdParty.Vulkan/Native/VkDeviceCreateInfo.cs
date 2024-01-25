namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDeviceCreateInfo.html">VkDeviceCreateInfo</see>
/// </summary>
public unsafe struct VkDeviceCreateInfo
{
    public readonly VkStructureType sType;
    public void*                     pNext;
    public VkDeviceCreateFlags       flags;
    public uint                      queueCreateInfoCount;
    public VkDeviceQueueCreateInfo*  pQueueCreateInfos;

    [Obsolete("Ignored")]
    public uint enabledLayerCount;

    [Obsolete("Ignored")]
    public byte** ppEnabledLayerNames;

    public uint                      enabledExtensionCount;
    public byte**                    ppEnabledExtensionNames;
    public VkPhysicalDeviceFeatures* pEnabledFeatures;

    public VkDeviceCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO;
}
