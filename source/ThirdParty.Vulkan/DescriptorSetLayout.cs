namespace ThirdParty.Vulkan;

public unsafe partial class DescriptorSetLayout : DisposableNativeHandle
{
    private readonly AllocationCallbacks? allocator;
    private readonly Device               device;

    internal DescriptorSetLayout(Device device, CreateInfo createInfo, AllocationCallbacks? allocator)
    {
        this.device    = device;
        this.allocator = allocator;

        fixed (VkDescriptorSetLayout* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateDescriptorSetLayout(device, createInfo, allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyDescriptorSetLayout(this.device, this, this.allocator);
}
