using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public unsafe class VkBuffer : VkDeviceResource<VkBuffer>
{
    internal VkBuffer(VkDevice device, in VkBufferCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkBuffer>* pHandle     = &this.handle)
        fixed (VkBufferCreateInfo* pCreateInfo = &createInfo)
        {
            fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
            {
                VkException.Check(PInvoke.vkCreateBuffer(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
            }
        }
    }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyBuffer(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }

    public void BindMemory(VkDeviceMemory memory, uint memoryOffset) =>
        VkException.Check(PInvoke.vkBindBufferMemory(this.Device.Handle, this.handle, memory.Handle, memoryOffset));

    public void GetMemoryRequirements(out VkMemoryRequirements memoryRequirements)
    {
        fixed (VkMemoryRequirements* pMemoryRequirements = &memoryRequirements)
        {
            PInvoke.vkGetBufferMemoryRequirements(this.Device.Handle, this.handle, pMemoryRequirements);
        }
    }
}
