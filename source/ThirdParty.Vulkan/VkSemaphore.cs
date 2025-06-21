using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe class VkSemaphore : VkDeviceResource<VkSemaphore>
{
    internal VkSemaphore(VkDevice device, in VkSemaphoreCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkSemaphore>* pHandle     = &this.handle)
        fixed (VkSemaphoreCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateSemaphore(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroySemaphore(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
