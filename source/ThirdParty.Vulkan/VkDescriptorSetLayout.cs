using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public unsafe partial class VkDescriptorSetLayout : DeviceResource<VkDescriptorSetLayout>
{
    internal VkDescriptorSetLayout(VkDevice device, in VkDescriptorSetLayoutCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkDescriptorSetLayout>* pHandle     = &this.handle)
        fixed (VkDescriptorSetLayoutCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*           pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateDescriptorSetLayout(device.Handle, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyDescriptorSetLayout(this.Device.Handle, this.handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
