using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public unsafe partial class VkImageView : VkDeviceResource<VkImageView>
{
    public VkImageView(VkDevice device, in VkImageViewCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkImageView>* pHandle     = &this.handle)
        fixed (VkImageViewCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateImageView(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyImageView(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
