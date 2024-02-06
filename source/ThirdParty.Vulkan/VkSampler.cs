using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkSampler : DeviceResource<VkSampler>
{
    internal VkSampler(VkDevice device, in VkSamplerCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkSampler>*   pHandle     = &this.Handle)
        fixed (VkSamplerCreateInfo*   pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateSampler(device, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroySampler(this.Device, this, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
