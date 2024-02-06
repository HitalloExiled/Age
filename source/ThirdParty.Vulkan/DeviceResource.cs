namespace ThirdParty.Vulkan;

public abstract class DeviceResource<T> : DisposableManagedHandle<T> where T : ManagedHandle<T>
{
    internal VkDevice Device { get; }

    internal VkInstance       Instance       => this.Device.PhysicalDevice.Instance;
    internal VkPhysicalDevice PhysicalDevice => this.Device.PhysicalDevice;

    internal DeviceResource(VkDevice device) : base() =>
        this.Device = device;

    internal DeviceResource(VkHandle<T> handle, VkDevice device) : base(handle) =>
        this.Device = device;
}
