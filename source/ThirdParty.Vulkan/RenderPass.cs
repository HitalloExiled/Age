namespace ThirdParty.Vulkan;

public unsafe partial class RenderPass : DeviceResource
{
    internal RenderPass(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkRenderPass* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateRenderPass(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyRenderPass(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
