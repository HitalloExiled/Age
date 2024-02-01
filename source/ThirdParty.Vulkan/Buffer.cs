using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Buffer : DisposableNativeHandle
{
    private readonly Device device;

    internal Buffer(Device device, CreateInfo createInfo)
    {
        this.device = device;

        fixed (VkBuffer* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateBuffer(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyBuffer(this.device, this.Handle, this.device.PhysicalDevice.Instance.Allocator);

    public void BindMemory(DeviceMemory memory, uint memoryOffset) =>
        VulkanException.Check(PInvoke.vkBindBufferMemory(this.device, this, memory, memoryOffset));

    public MemoryRequirements GetMemoryRequirements()
    {
        VkMemoryRequirements memoryRequirements;

        PInvoke.vkGetBufferMemoryRequirements(this.device, this, &memoryRequirements);

        return new(memoryRequirements);
    }


}
