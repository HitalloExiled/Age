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
            VkException.Check(PInvoke.vkCreateImage(device, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    public void BindMemory(VkDeviceMemory memory, ulong memoryOffset) =>
        VkException.Check(PInvoke.vkBindImageMemory(this.Device, this, memory, memoryOffset));

    public void GetMemoryRequirements(out VkMemoryRequirements memoryRequirements)
    {
        fixed (VkMemoryRequirements* pMemoryRequirements = &memoryRequirements)
        {
            PInvoke.vkGetImageMemoryRequirements(this.Device, this, pMemoryRequirements);
        }
    }

    protected override void OnDispose()
    {
        if (!this.reserved)
        {
            fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
            {
                PInvoke.vkDestroyImage(this.Device, this, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
            }
        }
    }
}
