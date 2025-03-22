using ThirdParty.Vulkan.Enums;

using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe partial class VkCommandPool : VkDeviceResource<VkCommandPool>
{
    public VkCommandPool(VkDevice device, in VkCommandPoolCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkCommandPool>* pHandle     = &this.handle)
        fixed (VkCommandPoolCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*   pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateCommandPool(device.Handle, pCreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyCommandPool(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }

    public VkCommandBuffer AllocateCommand(VkCommandBufferLevel level) =>
        this.AllocateCommands(1, level)[0];

    public VkCommandBuffer[] AllocateCommands(uint count, VkCommandBufferLevel level)
    {
        var allocateInfo = new VkCommandBufferAllocateInfo
        {
            CommandBufferCount = count,
            Level              = level,
        };

        return this.AllocateCommands(allocateInfo);
    }

    public VkCommandBuffer[] AllocateCommands(VkCommandBufferAllocateInfo allocateInfo)
    {
        allocateInfo.CommandPool = this.handle;

        var count = allocateInfo.CommandBufferCount;

        var buffer = new VkHandle<VkCommandBuffer>[count];

        fixed (VkHandle<VkCommandBuffer>* pBuffer = buffer)
        {
            VkException.Check(PInvoke.vkAllocateCommandBuffers(this.Device.Handle, &allocateInfo, pBuffer));
        }

        var commands = new VkCommandBuffer[count];

        for (var i = 0; i < count; i++)
        {
            commands[i] = new(buffer[i], this.Device, this);
        }

        return commands;
    }

    public void FreeCommandBuffers(scoped ReadOnlySpan<VkCommandBuffer> commandBuffers)
    {
        fixed (VkHandle<VkCommandPool>*   pHandle         = &this.handle)
        fixed (VkHandle<VkCommandBuffer>* pCommandBuffers = VkHandle.GetHandles(commandBuffers))
        {
            PInvoke.vkFreeCommandBuffers(this.Device.Handle, this.handle, (uint)commandBuffers.Length, pCommandBuffers);
        }
    }
}
