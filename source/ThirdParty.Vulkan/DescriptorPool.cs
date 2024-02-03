namespace ThirdParty.Vulkan;

public unsafe partial class DescriptorPool : DeviceResource
{
    internal DescriptorPool(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkDescriptorPool* pHandler = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateDescriptorPool(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandler));
        }
    }

    public DescriptorSet[] AllocateDescriptorSets(DescriptorSet.AllocateInfo allocInfo)
    {
        allocInfo.SetDescriptorPool(this);

        var vkDescriptorSets = new VkDescriptorSet[allocInfo.SetLayouts.Length];

        fixed (VkDescriptorSet* pDescriptorSets = vkDescriptorSets)
        {
            VulkanException.Check(PInvoke.vkAllocateDescriptorSets(this.Device, allocInfo, pDescriptorSets));
        }

        var descriptorSets = new DescriptorSet[vkDescriptorSets.Length];

        for (var i = 0; i < vkDescriptorSets.Length; i++)
        {
            descriptorSets[i] = new(vkDescriptorSets[i], this);
        }

        return descriptorSets;
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyDescriptorPool(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
