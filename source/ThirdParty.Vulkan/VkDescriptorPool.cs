using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkDescriptorPool : DeviceResource<VkDescriptorPool>
{
    internal VkDescriptorPool(VkDevice device, in VkDescriptorPoolCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkDescriptorPool>* pHandler    = &this.Handle)
        fixed (VkDescriptorPoolCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*      pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateDescriptorPool(device, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandler));
        }
    }

    public VkDescriptorSet[] AllocateDescriptorSets(VkDescriptorSetAllocateInfo allocInfo)
    {
        allocInfo.DescriptorPool = this;

        var vkDescriptorSets = new VkHandle<VkDescriptorSet>[allocInfo.DescriptorSetCount];

        fixed (VkHandle<VkDescriptorSet>* pDescriptorSets = vkDescriptorSets)
        {
            VkException.Check(PInvoke.vkAllocateDescriptorSets(this.Device, &allocInfo, pDescriptorSets));
        }

        var descriptorSets = new VkDescriptorSet[vkDescriptorSets.Length];

        for (var i = 0; i < vkDescriptorSets.Length; i++)
        {
            descriptorSets[i] = new(vkDescriptorSets[i], this);
        }

        return descriptorSets;
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyDescriptorPool(this.Device, this, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
