using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Image
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageSubresourceRange.html">VkImageSubresourceRange</see>
    /// </summary>
    public record SubresourceRange : NativeReference<VkImageSubresourceRange>
    {
        public VkImageAspectFlags AspectMask
        {
            get => this.PNative->aspectMask;
            init => this.PNative->aspectMask = value;
        }

        public uint BaseMipLevel
        {
            get => this.PNative->baseMipLevel;
            init => this.PNative->baseMipLevel = value;
        }

        public uint LevelCount
        {
            get => this.PNative->levelCount;
            init => this.PNative->levelCount = value;
        }

        public uint BaseArrayLayer
        {
            get => this.PNative->baseArrayLayer;
            init => this.PNative->baseArrayLayer = value;
        }

        public uint LayerCount
        {
            get => this.PNative->layerCount;
            init => this.PNative->layerCount = value;
        }
    }
}
