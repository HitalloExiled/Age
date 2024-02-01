using ThirdParty.Vulkan.Extensions.EXT;

namespace ThirdParty.Vulkan.EXT;

public unsafe partial class DebugUtilsMessenger : DisposableNativeHandle
{
    private readonly DebugUtilsExtension extension;

    internal DebugUtilsMessenger(VkDebugUtilsMessengerEXT handle, DebugUtilsExtension extension) : base(handle) =>
        this.extension = extension;

    protected override void OnDispose() =>
        this.extension.DestroyDebugUtilsMessenger(this);
}
