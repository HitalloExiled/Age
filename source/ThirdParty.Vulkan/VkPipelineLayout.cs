using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkPipelineLayout : DeviceResource<VkPipelineLayout>
{
    internal VkPipelineLayout(VkDevice device, in VkPipelineLayoutCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkPipelineLayout>* pHandle     = &this.Handle)
        fixed (VkPipelineLayoutCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*      pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreatePipelineLayout(device, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyPipelineLayout(this.Device, this, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
