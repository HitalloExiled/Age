namespace ThirdParty.Vulkan.Interfaces;

public interface IDeviceExtension<T> where T : IDeviceExtension<T>
{
    static abstract string Name { get; }
    static abstract T Create(Device device);
}
