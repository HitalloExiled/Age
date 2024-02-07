using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkGraphicsPipeline : VkPipeline
{
    internal VkGraphicsPipeline(VkHandle<VkPipeline> handle, VkDevice device) : base(handle, device) { }

    internal VkGraphicsPipeline(VkDevice device, in VkGraphicsPipelineCreateInfo createInfo, VkPipelineCache? pipelineCache) : base(device)
    {
        fixed (VkHandle<VkPipeline>*         pHandle     = &this.handle)
        fixed (VkGraphicsPipelineCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*        pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateGraphicsPipelines(device.Handle, pipelineCache?.Handle ?? default, 1, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }
}
