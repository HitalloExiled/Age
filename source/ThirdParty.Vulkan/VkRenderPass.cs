using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkRenderPass : DeviceResource<VkRenderPass>
{
    internal VkRenderPass(VkDevice device, in VkRenderPassCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkRenderPass>* pHandle     = &this.Handle)
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
            PInvoke.vkDestroyRenderPass(this.Device.Handle, this.Handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
