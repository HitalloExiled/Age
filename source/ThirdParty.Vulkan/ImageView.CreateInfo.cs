using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class ImageView
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageViewCreateInfo.html">VkImageViewCreateInfo</see>
    /// </summary>
    public record CreateInfo : NativeReference<VkImageViewCreateInfo>
    {
        private ComponentMapping?       components;
        private Image?                  image;
        private Image.SubresourceRange? subresourceRange;

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public ImageViewCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public Image? Image
        {
            get => this.image;
            init => this.PNative->image = this.image = value;
        }

        public ImageViewType ViewType
        {
            get => this.PNative->viewType;
            init => this.PNative->viewType = value;
        }

        public Format Format
        {
            get => this.PNative->format;
            init => this.PNative->format = value;
        }

        public ComponentMapping? Components
        {
            get => this.components;
            init => this.PNative->components = this.components = value;
        }

        public Image.SubresourceRange? SubresourceRange
        {
            get => this.subresourceRange;
            init => this.PNative->subresourceRange = this.subresourceRange = value;
        }
    }
}
