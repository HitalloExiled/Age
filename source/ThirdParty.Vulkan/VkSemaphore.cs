using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public unsafe partial class VkSemaphore : DeviceResource<VkSemaphore>
{
    internal VkSemaphore(VkDevice device, in VkSemaphoreCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkSemaphore>* pHandle     = &this.handle)
        fixed (VkSemaphoreCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateSemaphore(device.Handle, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroySemaphore(this.Device.Handle, this.handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
