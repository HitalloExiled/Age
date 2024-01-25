namespace ThirdParty.Vulkan.Native.KHR;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSwapchainCreateInfoKHR.html">VkSwapchainCreateInfoKHR</see>
/// </summary>
public unsafe struct VkSwapchainCreateInfoKHR
{
    public readonly VkStructureType sType;

    public void*                         pNext;
    public VkSwapchainCreateFlagsKHR     flags;
    public VkSurfaceKHR                  surface;
    public uint                          minImageCount;
    public VkFormat                      imageFormat;
    public VkColorSpaceKHR               imageColorSpace;
    public VkExtent2D                    imageExtent;
    public uint                          imageArrayLayers;
    public VkImageUsageFlags             imageUsage;
    public VkSharingMode                 imageSharingMode;
    public uint                          queueFamilyIndexCount;
    public uint*                         pQueueFamilyIndices;
    public VkSurfaceTransformFlagBitsKHR preTransform;
    public VkCompositeAlphaFlagBitsKHR   compositeAlpha;
    public VkPresentModeKHR              presentMode;
    public VkBool32                      clipped;
    public VkSwapchainKHR                oldSwapchain;

    public VkSwapchainCreateInfoKHR() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR;
}
