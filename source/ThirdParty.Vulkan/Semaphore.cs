namespace ThirdParty.Vulkan;

public unsafe partial class Semaphore : DeviceResource
{
    internal Semaphore(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkSemaphore* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateSemaphore(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroySemaphore(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
