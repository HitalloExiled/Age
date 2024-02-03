using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class Image : DeviceResource
{
    private readonly bool reserved;

    internal Image(VkImage handle, Device device, bool reserved) : base(handle, device) =>
        this.reserved = reserved;

    internal Image(Device device, CreateInfo createInfo, bool reserved) : base(device)
    {
        this.reserved = reserved;

        fixed (VkImage* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateImage(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    public void BindMemory(DeviceMemory memory, ulong memoryOffset) =>
        VulkanException.Check(PInvoke.vkBindImageMemory(this.Device, this, memory, memoryOffset));

    public MemoryRequirements GetMemoryRequirements()
    {
        VkMemoryRequirements memoryRequirements;

        PInvoke.vkGetImageMemoryRequirements(this.Device, this, &memoryRequirements);

        return new(memoryRequirements);
    }

    protected override void OnDispose()
    {
        if (!this.reserved)
        {
            PInvoke.vkDestroyImage(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
        }
    }
}
