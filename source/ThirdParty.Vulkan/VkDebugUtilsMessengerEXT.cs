using ThirdParty.Vulkan.Extensions;

namespace ThirdParty.Vulkan;

public sealed unsafe class VkDebugUtilsMessengerEXT : DisposableManagedHandle<VkDebugUtilsMessengerEXT>
{
    private readonly VkDebugUtilsExtensionEXT extension;

    internal VkDebugUtilsMessengerEXT(VkHandle<VkDebugUtilsMessengerEXT> handle, VkDebugUtilsExtensionEXT extension) : base(handle) =>
        this.extension = extension;

    protected override void Disposed() =>
        this.extension.DestroyDebugUtilsMessenger(this);
}
