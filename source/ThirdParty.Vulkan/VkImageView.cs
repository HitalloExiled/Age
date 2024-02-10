using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public unsafe partial class VkImageView : VkDeviceResource<VkImageView>
{
    public VkImageView(VkDevice device, in VkImageViewCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkImageView>* pHandle     = &this.handle)
        fixed (VkImageViewCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateImageView(device.Handle, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyImageView(this.Device.Handle, this.handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
