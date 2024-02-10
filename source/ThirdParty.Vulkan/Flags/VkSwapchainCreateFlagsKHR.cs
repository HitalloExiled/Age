namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSwapchainCreateFlagBitsKHR.html">VkSwapchainCreateFlagBitsKHR</see>
/// </summary>
[Flags]
public enum VkSwapchainCreateFlagsKHR
{
    SplitInstanceBindRegions = 0x00000001,
    Protected                = 0x00000002,
    MutableFormat            = 0x00000004,
    DeferredMemoryAllocation = 0x00000008,
}
