using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Image : DisposableNativeHandle
{
    private readonly Device device;

    internal Image(VkImage handle, Device device) : base(handle) =>
        this.device = device;

    internal Image(Device device, CreateInfo createInfo)
    {
        this.device = device;

        fixed (VkImage* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateImage(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
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
        PInvoke.vkDestroyImage(this.device, this, this.device.PhysicalDevice.Instance.Allocator);
}
