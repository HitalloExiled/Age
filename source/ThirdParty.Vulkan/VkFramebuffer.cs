using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public unsafe partial class VkFramebuffer : VkDeviceResource<VkFramebuffer>
{
    internal VkFramebuffer(VkDevice device, in VkFramebufferCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkFramebuffer>* pHandle     = &this.handle)
        fixed (VkFramebufferCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*   pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateFramebuffer(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyFramebuffer(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
