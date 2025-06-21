using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe class VkDescriptorSetLayout : VkDeviceResource<VkDescriptorSetLayout>
{
    internal VkDescriptorSetLayout(VkDevice device, in VkDescriptorSetLayoutCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkDescriptorSetLayout>* pHandle     = &this.handle)
        fixed (VkDescriptorSetLayoutCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*           pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateDescriptorSetLayout(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyDescriptorSetLayout(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
