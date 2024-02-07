using Age.Core.Interop;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkCommandPool : DeviceResource<VkCommandPool>
{
    public VkCommandPool(VkDevice device, in VkCommandPoolCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkCommandPool>* pHandle     = &this.Handle)
        fixed (VkCommandPoolCreateInfo* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*   pAllocator  = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateCommandPool(device.Handle, pCreateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyCommandPool(this.Device.Handle, this.Handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }

    public VkCommandBuffer AllocateCommand(VkCommandBufferLevelFlags level) =>
        this.AllocateCommands(1, level)[0];

    public VkCommandBuffer[] AllocateCommands(uint count, VkCommandBufferLevelFlags level)
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
        allocateInfo.CommandPool = this.Handle;

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

    public void FreeCommandBuffers(params VkCommandBuffer[] commandBuffers)
    {
        fixed (VkHandle<VkCommandPool>*   pHandle         = &this.Handle)
        fixed (VkHandle<VkCommandBuffer>* pCommandBuffers = commandBuffers.Select(x => x.Handle).ToArray())
        {
            PInvoke.vkFreeCommandBuffers(this.Device.Handle, this.Handle, (uint)commandBuffers.Length, pCommandBuffers);
        }
    }
}
