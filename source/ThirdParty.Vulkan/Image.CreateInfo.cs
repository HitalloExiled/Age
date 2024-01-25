using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Image
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageCreateInfo.html">VkImageCreateInfo</see>
    /// </summary>
    public record CreateInfo : NativeReference<VkImageCreateInfo>
    {
        private Extent3D? extent;
        private uint[]    queueFamilyIndices = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public ImageCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public ImageType ImageType
        {
            get => this.PNative->imageType;
            init => this.PNative->imageType = value;
        }

        public Format Format
        {
            get => this.PNative->format;
            init => this.PNative->format = value;
        }

        public Extent3D? Extent
        {
            get => this.extent;
            init => this.PNative->extent = this.extent = value;
        }

        public uint MipLevels
        {
            get => this.PNative->mipLevels;
            init => this.PNative->mipLevels = value;
        }

        public uint ArrayLayers
        {
            get => this.PNative->arrayLayers;
            init => this.PNative->arrayLayers = value;
        }

        public SampleCountFlags Samples
        {
            get => this.PNative->samples;
            init => this.PNative->samples = value;
        }

        public ImageTiling Tiling
        {
            get => this.PNative->tiling;
            init => this.PNative->tiling = value;
        }

        public ImageUsageFlags Usage
        {
            get => this.PNative->usage;
            init => this.PNative->usage = value;
        }

        public SharingMode SharingMode
        {
            get => this.PNative->sharingMode;
            init => this.PNative->sharingMode = value;
        }

        public uint[] QueueFamilyIndices
        {
            get => this.queueFamilyIndices;
            init => Init(ref this.queueFamilyIndices, ref this.PNative->pQueueFamilyIndices, ref this.PNative->queueFamilyIndexCount, value);
        }

        public ImageLayout InitialLayout
        {
            get => this.PNative->initialLayout;
            init => this.PNative->initialLayout = value;
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pQueueFamilyIndices);
    }
}
