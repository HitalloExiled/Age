using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Buffer : DeviceResource
{
    internal Buffer(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkBuffer* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateBuffer(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyBuffer(this.Device, this.Handle, this.Device.PhysicalDevice.Instance.Allocator);

    public void BindMemory(DeviceMemory memory, uint memoryOffset) =>
        VulkanException.Check(PInvoke.vkBindBufferMemory(this.Device, this, memory, memoryOffset));

    public MemoryRequirements GetMemoryRequirements()
    {
        VkMemoryRequirements memoryRequirements;

        PInvoke.vkGetBufferMemoryRequirements(this.Device, this, &memoryRequirements);

        return new(memoryRequirements);
    }


}
