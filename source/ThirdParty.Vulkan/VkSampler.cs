using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe class VkSampler : VkDeviceResource<VkSampler>
{
    internal VkSampler(VkDevice device, in VkSamplerCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkSampler>*   pHandle     = &this.handle)
        fixed (VkSamplerCreateInfo*   pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateSampler(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroySampler(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
