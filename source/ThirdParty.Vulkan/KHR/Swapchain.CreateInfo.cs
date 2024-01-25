using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Flags.KHR;
using ThirdParty.Vulkan.Native.KHR;

namespace ThirdParty.Vulkan.KHR;

public partial class Swapchain
{
    /// <inheritdoc cref="VkSwapchainCreateInfoKHR" />
    public unsafe record CreateInfo : NativeReference<VkSwapchainCreateInfoKHR>
    {
        private Surface?   surface;
        private Extent2D?  imageExtent;
        private uint[]     queueFamilyIndices = [];
        private Swapchain? oldSwapchain;

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public SwapchainCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public Surface? Surface
        {
            get => this.surface;
            init => this.PNative->surface = this.surface = value;
        }

        public uint MinImageCount
        {
            get => this.PNative->minImageCount;
            init => this.PNative->minImageCount = value;
        }

        public Format ImageFormat
        {
            get => this.PNative->imageFormat;
            init => this.PNative->imageFormat = value;
        }

        public ColorSpace ImageColorSpace
        {
            get => this.PNative->imageColorSpace;
            init => this.PNative->imageColorSpace = value;
        }

        public Extent2D? ImageExtent
        {
            get => this.imageExtent;
            init => this.PNative->imageExtent = this.imageExtent = value;
        }

        public uint ImageArrayLayers
        {
            get => this.PNative->imageArrayLayers;
            init => this.PNative->imageArrayLayers = value;
        }

        public ImageUsageFlags ImageUsage
        {
            get => this.PNative->imageUsage;
            init => this.PNative->imageUsage = value;
        }

        public SharingMode ImageSharingMode
        {
            get => this.PNative->imageSharingMode;
            init => this.PNative->imageSharingMode = value;
        }

        public uint[] QueueFamilyIndices
        {
            get => this.queueFamilyIndices;
            init => Init(ref this.queueFamilyIndices, ref this.PNative->pQueueFamilyIndices, ref this.PNative->queueFamilyIndexCount, value);
        }

        public SurfaceTransformFlags PreTransform
        {
            get => this.PNative->preTransform;
            init => this.PNative->preTransform = value;
        }

        public CompositeAlphaFlags CompositeAlpha
        {
            get => this.PNative->compositeAlpha;
            init => this.PNative->compositeAlpha = value;
        }

        public PresentMode PresentMode
        {
            get => this.PNative->presentMode;
            init => this.PNative->presentMode = value;
        }

        public bool Clipped
        {
            get => this.PNative->clipped;
            init => this.PNative->clipped = value;
        }

        public Swapchain? OldSwapchain
        {
            get => this.oldSwapchain;
            init => this.PNative->oldSwapchain = this.oldSwapchain = value;
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pQueueFamilyIndices);
    }
}
