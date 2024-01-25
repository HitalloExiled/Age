using ThirdParty.Vulkan.Native.EXT;

namespace ThirdParty.Vulkan.EXT;

public unsafe partial class DebugUtilsMessenger
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDebugUtilsMessengerCallbackDataEXT.html">VkDebugUtilsMessengerCallbackDataEXT</see>
    /// </summary>
    public unsafe record CallbackData : NativeReference<VkDebugUtilsMessengerCallbackDataEXT>
    {
        private VkDebugUtilsLabelEXT[]          cmdBufLabels = [];
        private string?                         message;
        private string?                         messageIdName;
        private VkDebugUtilsObjectNameInfoEXT[] objects = [];
        private VkDebugUtilsLabelEXT[]          queueLabels = [];

        public VkDebugUtilsLabelEXT[]          CmdBufLabels    => Get(ref this.cmdBufLabels, this.PNative->pQueueLabels, this.PNative->cmdBufLabelCount);
        public uint                            Flags           => this.PNative->flags;
        public string?                         Message         => Get(ref this.message, this.PNative->pMessage);
        public string?                         MessageIdName   => Get(ref this.messageIdName, this.PNative->pMessageIdName);
        public int                             MessageIdNumber => this.PNative->messageIdNumber;
        public nint                            Next            => (nint)this.PNative->pNext;
        public VkDebugUtilsObjectNameInfoEXT[] Objects         => Get(ref this.objects, this.PNative->pObjects, this.PNative->objectCount);
        public VkDebugUtilsLabelEXT[]          QueueLabels     => Get(ref this.queueLabels, this.PNative->pQueueLabels, this.PNative->queueLabelCount);

        internal CallbackData(in VkDebugUtilsMessengerCallbackDataEXT value) : base(value) { }
    }
}
