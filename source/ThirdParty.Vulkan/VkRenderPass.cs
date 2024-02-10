using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public unsafe partial class VkRenderPass : VkDeviceResource<VkRenderPass>
{
    internal VkRenderPass(VkDevice device, in VkRenderPassCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkRenderPass>* pHandle     = &this.handle)
        fixed (VkRenderPassCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*  pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateRenderPass(device.Handle, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyRenderPass(this.Device.Handle, this.handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
