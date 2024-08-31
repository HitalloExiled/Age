#pragma warning disable IDE0001

using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkInstanceCreateInfo.html">VkInstanceCreateInfo</see>
/// </summary>
public unsafe struct VkInstanceCreateInfo
{
    public readonly VkStructureType SType;

    public void*                 PNext;
    public VkInstanceCreateFlags Flags;
    public VkApplicationInfo*    PApplicationInfo;
    public uint                  EnabledLayerCount;
    public byte**                PpEnabledLayerNames;
    public uint                  EnabledExtensionCount;
    public byte**                PpEnabledExtensionNames;

    public VkInstanceCreateInfo() =>
        this.SType = VkStructureType.InstanceCreateInfo;
}
