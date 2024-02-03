namespace ThirdParty.Vulkan;

public unsafe partial class GraphicsPipeline : Pipeline
{
    internal GraphicsPipeline(VkPipeline handle, Device device) : base(handle, device) { }

    internal GraphicsPipeline(Device device, CreateInfo createInfo, PipelineCache? pipelineCache) : base(device)
    {
        fixed (VkPipeline* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateGraphicsPipelines(device, pipelineCache, 1, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }
}
