using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Flags.KHR;
using ThirdParty.Vulkan.KHR;

namespace ThirdParty.Vulkan.Native.KHR;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSwapchainCreateInfoKHR.html">VkSwapchainCreateInfoKHR</see>
/// </summary>
public unsafe struct VkSwapchainCreateInfoKHR
{
    public readonly VkStructureType SType;

    public void*                      PNext;
    public VkSwapchainCreateFlagsKHR  Flags;
    public VkHandle<VkSurfaceKHR>     Surface;
    public uint                       MinImageCount;
    public VkFormat                   ImageFormat;
    public VkColorSpaceKHR            ImageColorSpace;
    public VkExtent2D                 ImageExtent;
    public uint                       ImageArrayLayers;
    public VkImageUsageFlags          ImageUsage;
    public VkSharingMode              ImageSharingMode;
    public uint                       QueueFamilyIndexCount;
    public uint*                      PQueueFamilyIndices;
    public VkSurfaceTransformFlagsKHR PreTransform;
    public VkCompositeAlphaFlagsKHR   CompositeAlpha;
    public VkPresentModeKHR           PresentMode;
    public VkBool32                   Clipped;
    public VkHandle<VkSwapchainKHR>   OldSwapchain;

    public VkSwapchainCreateInfoKHR() =>
        this.SType = VkStructureType.SwapchainCreateInfoKHR;
}
