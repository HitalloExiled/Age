using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public class Win32SurfaceExtension : IInstanceExtension<Win32SurfaceExtension>
{
    public static string Name => throw new NotImplementedException();

    static Win32SurfaceExtension IInstanceExtension<Win32SurfaceExtension>.Create(Instance instance) => throw new NotImplementedException();

    public Surface CreateSurface(Win32Surface.CreateInfo createInfo) => throw new NotImplementedException();
}
