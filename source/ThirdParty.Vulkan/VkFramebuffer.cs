using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkFramebuffer : DeviceResource<VkFramebuffer>
{
    internal VkFramebuffer(VkDevice device, in VkFramebufferCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkFramebuffer>* pHandle     = &this.Handle)
        fixed (VkFramebufferCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*   pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateFramebuffer(device.Handle, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyFramebuffer(this.Device.Handle, this.Handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
