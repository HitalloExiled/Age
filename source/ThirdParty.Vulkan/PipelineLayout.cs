namespace ThirdParty.Vulkan;

public unsafe partial class PipelineLayout : DeviceResource
{
    internal PipelineLayout(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkPipelineLayout* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreatePipelineLayout(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyPipelineLayout(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
