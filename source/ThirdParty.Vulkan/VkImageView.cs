using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkImageView : DeviceResource<VkImageView>
{
    public VkImageView(VkDevice device, in VkImageViewCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkImageView>* pHandle     = &this.Handle)
        fixed (VkImageViewCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateImageView(device, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyImageView(this.Device, this, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
