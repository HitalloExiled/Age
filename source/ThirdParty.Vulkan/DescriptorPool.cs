namespace ThirdParty.Vulkan;

public unsafe partial class DescriptorPool : DisposableNativeHandle
{
    private readonly Device device;

    internal DescriptorPool(Device device, CreateInfo createInfo)
    {
        this.device = device;

        fixed (VkDescriptorPool* pHandler = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateDescriptorPool(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandler));
        }
    }

    public DescriptorSet[] AllocateDescriptorSets(DescriptorSet.AllocateInfo allocInfo) => throw new NotImplementedException();
    protected override void OnDispose() =>
        PInvoke.vkDestroyDescriptorPool(this.device, this, this.device.PhysicalDevice.Instance.Allocator);
}
