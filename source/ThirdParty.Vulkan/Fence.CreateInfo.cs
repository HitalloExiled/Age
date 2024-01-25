using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Fence
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFenceCreateInfo.html">VkFenceCreateInfo</see>
    /// </summary>
    public record CreateInfo : NativeReference<VkFenceCreateInfo>
    {
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public FenceCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }
    }
}
