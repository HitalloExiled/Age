using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe partial class VkImage : VkDeviceResource<VkImage>
{
    private readonly bool reserved;

    internal VkImage(VkHandle<VkImage> handle, VkDevice device, bool reserved) : base(handle, device) =>
        this.reserved = reserved;

    internal VkImage(VkDevice device, in VkImageCreateInfo createInfo, bool reserved) : base(device)
    {
        this.reserved = reserved;

        fixed (VkHandle<VkImage>*     pHandle     = &this.handle)
        fixed (VkImageCreateInfo*     pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateImage(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    public void BindMemory(VkDeviceMemory memory, ulong memoryOffset) =>
        VkException.Check(PInvoke.vkBindImageMemory(this.Device.Handle, this.handle, memory.Handle, memoryOffset));

    public void GetMemoryRequirements(out VkMemoryRequirements memoryRequirements)
    {
        fixed (VkMemoryRequirements* pMemoryRequirements = &memoryRequirements)
        {
            PInvoke.vkGetImageMemoryRequirements(this.Device.Handle, this.handle, pMemoryRequirements);
        }
    }

    protected override void Disposed()
    {
        if (!this.reserved)
        {
            fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
            {
                PInvoke.vkDestroyImage(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
            }
        }
    }
}
