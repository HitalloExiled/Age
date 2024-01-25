using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public class SurfaceExtension : IInstanceExtension<SurfaceExtension>
{
    public static string Name => throw new NotImplementedException();

    static SurfaceExtension IInstanceExtension<SurfaceExtension>.Create(Instance instance) => throw new NotImplementedException();
    public bool GetPhysicalDeviceSurfaceSupport(PhysicalDevice physicalDevice, uint i, Surface surface) => throw new NotImplementedException();
}
