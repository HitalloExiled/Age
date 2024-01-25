using ThirdParty.Vulkan.EXT;
using ThirdParty.Vulkan.Interfaces;

namespace ThirdParty.Vulkan.Extensions.EXT;

public class DebugUtilsExtension : IInstanceExtension<DebugUtilsExtension>
{
    public static string Name => throw new NotImplementedException();

    public static DebugUtilsExtension Create(Instance instance) => throw new NotImplementedException();
    public DebugUtilsMessenger CreateDebugUtilsMessenger(DebugUtilsMessenger.CreateInfo createInfo) => throw new NotImplementedException();
}
