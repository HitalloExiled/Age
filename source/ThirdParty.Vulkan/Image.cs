using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Image : DisposableNativeHandle
{
    private readonly AllocationCallbacks? allocator;
    private readonly Device               device;

    public Image(Device device, CreateInfo createInfo, AllocationCallbacks? allocator)
    {
        this.device    = device;
        this.allocator = allocator;

        fixed (VkImage* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateImage(device, createInfo, allocator, pHandle));
        }
    }

    public void BindMemory(DeviceMemory memory, ulong memoryOffset) =>
        VulkanException.Check(PInvoke.vkBindImageMemory(this.device, this, memory, memoryOffset));

    public MemoryRequirements GetMemoryRequirements()
    {
        VkMemoryRequirements memoryRequirements;

        PInvoke.vkGetImageMemoryRequirements(this.device, this, &memoryRequirements);

        return new(memoryRequirements);
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyImage(this.device, this, this.allocator);
}
