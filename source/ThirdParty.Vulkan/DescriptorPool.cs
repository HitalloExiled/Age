namespace ThirdParty.Vulkan;

public unsafe partial class DescriptorPool : DisposableNativeHandle
{
    private readonly Device               device;
    private readonly AllocationCallbacks? allocator;

    internal DescriptorPool(Device device, CreateInfo createInfo, AllocationCallbacks? allocator)
    {
        this.device    = device;
        this.allocator = allocator;

        fixed (VkDescriptorPool* pHandler = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateDescriptorPool(device, createInfo, allocator, pHandler));
        }
    }

    public DescriptorSet[] AllocateDescriptorSets(DescriptorSet.AllocateInfo allocInfo) => throw new NotImplementedException();
    protected override void OnDispose() =>
        PInvoke.vkDestroyDescriptorPool(this.device, this, this.allocator);
}
