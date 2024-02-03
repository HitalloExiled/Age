using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Instance
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkInstanceCreateInfo.html">VkInstanceCreateInfo</see>
    /// </summary>
    public record CreateInfo : NativeReference<VkInstanceCreateInfo>
    {
        private ApplicationInfo? applicationInfo;
        private string[]         enabledLayers     = [];
        private string[]         enabledExtensions = [];

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public InstanceCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public ApplicationInfo? ApplicationInfo
        {
            get => this.applicationInfo;
            init => this.PNative->pApplicationInfo = this.applicationInfo = value;
        }

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

        protected override void OnFinalize()
        {
            Free(ref this.PNative->ppEnabledLayerNames,     this.PNative->enabledLayerCount);
            Free(ref this.PNative->ppEnabledExtensionNames, this.PNative->enabledExtensionCount);
        }
    }
}
