using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe partial class VkRenderPass : VkDeviceResource<VkRenderPass>
{
    internal VkRenderPass(VkDevice device, in VkRenderPassCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkRenderPass>* pHandle     = &this.handle)
        fixed (VkRenderPassCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*  pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateRenderPass(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyRenderPass(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
