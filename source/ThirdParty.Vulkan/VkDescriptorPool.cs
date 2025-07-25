using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe class VkDescriptorPool : VkDeviceResource<VkDescriptorPool>
{
    internal VkDescriptorPool(VkDevice device, in VkDescriptorPoolCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkDescriptorPool>* pHandler    = &this.handle)
        fixed (VkDescriptorPoolCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*      pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateDescriptorPool(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandler));
        }
    }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyDescriptorPool(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }

    public VkDescriptorSet[] AllocateDescriptorSets(VkDescriptorSetAllocateInfo allocInfo)
    {
        allocInfo.DescriptorPool = this.handle;

        var vkDescriptorSets = new VkHandle<VkDescriptorSet>[allocInfo.DescriptorSetCount];

        fixed (VkHandle<VkDescriptorSet>* pDescriptorSets = vkDescriptorSets)
        {
            VkException.Check(PInvoke.vkAllocateDescriptorSets(this.Device.Handle, &allocInfo, pDescriptorSets));
        }

        var descriptorSets = new VkDescriptorSet[vkDescriptorSets.Length];

        for (var i = 0; i < vkDescriptorSets.Length; i++)
        {
            descriptorSets[i] = new(vkDescriptorSets[i], this);
        }

        return descriptorSets;
    }
}
