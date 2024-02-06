using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe class VkBuffer : DeviceResource<VkBuffer>
{
    internal VkBuffer(VkDevice device, in VkBufferCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkBuffer>* pHandle     = &this.Handle)
        fixed (VkBufferCreateInfo* pCreateInfo = &createInfo)
        {
            fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
            {
                VkException.Check(PInvoke.vkCreateBuffer(device, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
            }
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyBuffer(this.Device, this.Handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }

    public void BindMemory(VkDeviceMemory memory, uint memoryOffset) =>
        VkException.Check(PInvoke.vkBindBufferMemory(this.Device, this, memory, memoryOffset));

    public void GetMemoryRequirements(out VkMemoryRequirements memoryRequirements)
    {
        fixed (VkMemoryRequirements* pMemoryRequirements = &memoryRequirements)
        {
            PInvoke.vkGetBufferMemoryRequirements(this.Device, this, pMemoryRequirements);
        }
    }
}
