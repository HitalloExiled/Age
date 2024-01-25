using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class CommandBuffer
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandBufferBeginInfo.html">VkCommandBufferBeginInfo</see>
    /// </summary>
    public unsafe record BeginInfo : NativeReference<VkCommandBufferBeginInfo>
    {
        private InheritanceInfo? inheritanceInfo;

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public CommandBufferUsageFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public InheritanceInfo? InheritanceInfo
        {
            get => this.inheritanceInfo;
            init => this.PNative->pInheritanceInfo = this.inheritanceInfo = value;
        }
    }
}
