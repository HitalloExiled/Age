using ThirdParty.Vulkan.Extensions.EXT;

namespace ThirdParty.Vulkan.EXT;

public unsafe partial class VkDebugUtilsMessengerEXT : DisposableManagedHandle<VkDebugUtilsMessengerEXT>
{
    private readonly VkDebugUtilsExtensionEXT extension;

    internal VkDebugUtilsMessengerEXT(VkHandle<VkDebugUtilsMessengerEXT> handle, VkDebugUtilsExtensionEXT extension) : base(handle) =>
        this.extension = extension;

    protected override void OnDispose() =>
        this.extension.DestroyDebugUtilsMessenger(this);
}
