using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class CommandPool
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkCommandPoolCreateInfo.html">VkCommandPoolCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkCommandPoolCreateInfo>
    {
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public CommandPoolCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public uint QueueFamilyIndex
        {
            get => this.PNative->queueFamilyIndex;
            init => this.PNative->queueFamilyIndex = value;
        }
    }
}
