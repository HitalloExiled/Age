namespace ThirdParty.Vulkan;

public abstract class VkDeviceResource<T> : DisposableManagedHandle<T> where T : ManagedHandle<T>
{
    internal VkDevice Device { get; }

    internal VkInstance       Instance       => this.Device.PhysicalDevice.Instance;
    internal VkPhysicalDevice PhysicalDevice => this.Device.PhysicalDevice;

    internal VkDeviceResource(VkDevice device) =>
        this.Device = device;

    internal VkDeviceResource(VkHandle<T> handle, VkDevice device) : base(handle) =>
        this.Device = device;
}
