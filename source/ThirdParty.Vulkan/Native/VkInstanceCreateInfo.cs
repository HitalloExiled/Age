#pragma warning disable IDE0001

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkInstanceCreateInfo.html">VkInstanceCreateInfo</see>
/// </summary>
public unsafe struct VkInstanceCreateInfo
{
    public readonly VkStructureType sType;

    public void*                 pNext;
    public VkInstanceCreateFlags flags;
    public VkApplicationInfo*    pApplicationInfo;
    public uint                  enabledLayerCount;
    public byte**                ppEnabledLayerNames;
    public uint                  enabledExtensionCount;
    public byte**                ppEnabledExtensionNames;

    public VkInstanceCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO;
}
