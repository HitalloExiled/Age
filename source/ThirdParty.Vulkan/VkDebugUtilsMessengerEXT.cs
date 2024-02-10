using ThirdParty.Vulkan.Extensions;

namespace ThirdParty.Vulkan;

public unsafe partial class VkDebugUtilsMessengerEXT : DisposableManagedHandle<VkDebugUtilsMessengerEXT>
{
    private readonly VkDebugUtilsExtensionEXT extension;

    internal VkDebugUtilsMessengerEXT(VkHandle<VkDebugUtilsMessengerEXT> handle, VkDebugUtilsExtensionEXT extension) : base(handle) =>
        this.extension = extension;

    protected override void OnDispose() =>
        this.extension.DestroyDebugUtilsMessenger(this);
}
