using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public unsafe partial class VkSampler : VkDeviceResource<VkSampler>
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

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroySampler(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
