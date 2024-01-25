using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

#pragma warning disable CS0618

public unsafe partial class Device
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDeviceCreateInfo.html">VkDeviceCreateInfo</see>
    /// </summary>
    public record CreateInfo : NativeReference<VkDeviceCreateInfo>
    {
        private string[]                 enabledExtensions = [];
        private PhysicalDevice.Features? enabledFeatures;
        private string[]                 enabledLayers    = [];
        private DeviceQueue.CreateInfo[] queueCreateInfos = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public uint Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public DeviceQueue.CreateInfo[] QueueCreateInfos
        {
            get => this.queueCreateInfos;
            init => Init(ref this.queueCreateInfos, ref this.PNative->pQueueCreateInfos, ref this.PNative->queueCreateInfoCount, value);
        }

        [Obsolete("Ignored")]
        public string[] EnabledLayers
        {
            get => this.enabledLayers;
            init => Init(ref this.enabledLayers, ref this.PNative->ppEnabledLayerNames, ref this.PNative->enabledLayerCount, value);
        }

        public string[] EnabledExtensions
        {
            get => this.enabledExtensions;
            init => Init(ref this.enabledExtensions, ref this.PNative->ppEnabledExtensionNames, ref this.PNative->enabledExtensionCount, value);
        }

        public PhysicalDevice.Features? EnabledFeatures
        {
            get => this.enabledFeatures;
            init => this.PNative->pEnabledFeatures = this.enabledFeatures = value;
        }

        protected override void OnFinalize()
        {
            Free(ref this.PNative->ppEnabledExtensionNames, this.PNative->enabledExtensionCount);
            Free(ref this.PNative->ppEnabledLayerNames,     this.PNative->enabledLayerCount);
            Free(ref this.PNative->pQueueCreateInfos);
        }
    }
}
