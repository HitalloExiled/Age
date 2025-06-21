using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe class VkFramebuffer : VkDeviceResource<VkFramebuffer>
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

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyFramebuffer(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
