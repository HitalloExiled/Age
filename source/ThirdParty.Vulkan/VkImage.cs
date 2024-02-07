using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkImage : DeviceResource<VkImage>
{
    private readonly bool reserved;

    internal VkImage(VkHandle<VkImage> handle, VkDevice device, bool reserved) : base(handle, device) =>
        this.reserved = reserved;

    internal VkImage(VkDevice device, in VkImageCreateInfo createInfo, bool reserved) : base(device)
    {
        this.reserved = reserved;

        fixed (VkHandle<VkImage>*     pHandle     = &this.Handle)
        fixed (VkImageCreateInfo*     pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateImage(device.Handle, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    public void BindMemory(VkDeviceMemory memory, ulong memoryOffset) =>
        VkException.Check(PInvoke.vkBindImageMemory(this.Device.Handle, this.Handle, memory.Handle, memoryOffset));

    public void GetMemoryRequirements(out VkMemoryRequirements memoryRequirements)
    {
        fixed (VkMemoryRequirements* pMemoryRequirements = &memoryRequirements)
        {
            PInvoke.vkGetImageMemoryRequirements(this.Device.Handle, this.Handle, pMemoryRequirements);
        }
    }

    protected override void OnDispose()
    {
        if (!this.reserved)
        {
            fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
            {
                PInvoke.vkDestroyImage(this.Device.Handle, this.Handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
            }
        }
    }
}
