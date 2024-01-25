
using ThirdParty.Vulkan.Enums.KHR;

namespace ThirdParty.Vulkan.KHR;

#pragma warning disable IDE0001

public class Surface : DisposableNativeHandle
{
    protected override void OnDispose() => throw new NotImplementedException();

    public SurfaceCapabilities GetCapabilities(PhysicalDevice physicalDevice) => throw new NotImplementedException();
    public SurfaceFormat[] GetFormats(PhysicalDevice physicalDevice) => throw new NotImplementedException();
    public PresentMode[] GetPresentModes(PhysicalDevice physicalDevice) => throw new NotImplementedException();
}
