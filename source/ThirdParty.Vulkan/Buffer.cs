using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Buffer : DisposableNativeHandle
{
    private readonly Device device;
    private readonly AllocationCallbacks? allocator;

    internal Buffer(Device device, CreateInfo createInfo, AllocationCallbacks? allocator)
    {
        fixed (VkBuffer* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateBuffer(device, createInfo, allocator, pHandle));
        }

        this.device    = device;
        this.allocator = allocator;
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyBuffer(this.device, this.Handle, this.allocator);

    public void BindMemory(DeviceMemory memory, uint memoryOffset) =>
        VulkanException.Check(PInvoke.vkBindBufferMemory(this.device, this, memory, memoryOffset));

    public MemoryRequirements GetMemoryRequirements()
    {
        VkMemoryRequirements memoryRequirements;

        PInvoke.vkGetBufferMemoryRequirements(this.device, this, &memoryRequirements);

        return new(memoryRequirements);
    }


}
