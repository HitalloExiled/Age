using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class DeviceQueue
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDeviceQueueCreateInfo.html">VkDeviceQueueCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkDeviceQueueCreateInfo>
    {
        private float[] queuePriorities = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public DeviceQueueCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public uint QueueFamilyIndex
        {
            get => this.PNative->queueFamilyIndex;
            init => this.PNative->queueFamilyIndex = value;
        }

        public float[] QueuePriorities
        {
            get => this.queuePriorities;
            init => Init(ref this.queuePriorities, ref this.PNative->pQueuePriorities, ref this.PNative->queueCount, value);
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pQueuePriorities);
    }
}
