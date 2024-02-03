namespace ThirdParty.Vulkan;

public unsafe partial class ShaderModule : DeviceResource
{
    internal ShaderModule(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkShaderModule* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateShaderModule(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyShaderModule(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
