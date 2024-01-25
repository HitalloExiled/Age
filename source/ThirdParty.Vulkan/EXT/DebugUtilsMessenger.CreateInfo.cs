using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Flags.EXT;
using ThirdParty.Vulkan.Native.EXT;

namespace ThirdParty.Vulkan.EXT;

public unsafe partial class DebugUtilsMessenger
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsMessengerCreateInfoEXT.html">VkDebugUtilsMessengerCreateInfoEXT</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkDebugUtilsMessengerCreateInfoEXT>
    {
        /// <summary>
        /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkDebugUtilsMessengerCallbackEXT.html">PFN_vkDebugUtilsMessengerCallbackEXT</see>
        /// </summary>
        public unsafe delegate bool DebugUtilsMessengerCallback(DebugUtilsMessageSeverityFlags messageSeverity, DebugUtilsMessageTypeFlags messageTypes, CallbackData callbackData);

        private DebugUtilsMessengerCallback? callback;

        public uint Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public DebugUtilsMessageSeverityFlags MessageSeverity
        {
            get => this.PNative->messageSeverity;
            init => this.PNative->messageSeverity = value;
        }

        public DebugUtilsMessageTypeFlags MessageType
        {
            get => this.PNative->messageType;
            init => this.PNative->messageType = value;
        }


        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public DebugUtilsMessengerCallback? UserCallback
        {
            get => this.callback;
            set
            {
                this.PNative->pUserData       = (void*)ToPointer(ref this.callback, value);
                this.PNative->pfnUserCallback = Marshal.GetFunctionPointerForDelegate(NativeCallback);
            }
        }

        private static bool NativeCallback(VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
        {
            var callback = Marshal.GetDelegateForFunctionPointer<DebugUtilsMessengerCallback>((nint)pUserData);

            return callback(messageSeverity, messageTypes, new(*pCallbackData));
        }

        protected override void OnFinalize() { }
    }
}
