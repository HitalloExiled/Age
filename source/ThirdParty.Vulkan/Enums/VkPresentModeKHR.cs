namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPresentModeKHR.html">VkPresentModeKHR</see>
/// </summary>
public enum VkPresentModeKHR
{
    Immediate               = 0,
    Mailbox                 = 1,
    Fifo                    = 2,
    FifoRelaxed             = 3,
    SharedDemandRefresh     = 1000111000,
    SharedContinuousRefresh = 1000111001,
}
