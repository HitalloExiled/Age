using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public unsafe partial class VkPipelineLayout : VkDeviceResource<VkPipelineLayout>
{
    internal VkPipelineLayout(VkDevice device, in VkPipelineLayoutCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkPipelineLayout>* pHandle     = &this.handle)
        fixed (VkPipelineLayoutCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*      pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreatePipelineLayout(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyPipelineLayout(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}