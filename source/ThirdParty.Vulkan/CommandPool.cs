using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

public unsafe partial class CommandPool : DeviceResource
{
    public CommandPool(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkCommandPool* pValue = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateCommandPool(device, createInfo, device.PhysicalDevice.Instance.Allocator, pValue));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyCommandPool(this.Device, this.Handle, this.Device.PhysicalDevice.Instance.Allocator);

    #pragma warning disable IDE0001
    public CommandBuffer AllocateCommand(CommandBufferLevelFlags level) =>
        this.AllocateCommands(1, level)[0];

    public CommandBuffer[] AllocateCommands(uint count, CommandBufferLevelFlags level)
    {
        var allocateInfo = new CommandBuffer.AllocateInfo
        {
            CommandBufferCount = count,
            Level              = level,
        };

        return this.AllocateCommands(allocateInfo);
    }
    #pragma warning restore IDE0001

    public CommandBuffer[] AllocateCommands(CommandBuffer.AllocateInfo allocateInfo)
    {
        allocateInfo.SetCommandPool(this);

        var count = allocateInfo.CommandBufferCount;

        var buffer = new VkCommandBuffer[count];

        fixed (VkCommandBuffer* pBuffer = buffer)
        {
            VulkanException.Check(PInvoke.vkAllocateCommandBuffers(this.Device, allocateInfo, pBuffer));
        }

        var commands = new CommandBuffer[count];

        for (var i = 0; i < count; i++)
        {
            commands[i] = new(buffer[i], this.Device, this);
        }

        return commands;
    }

    public void FreeCommandBuffers(params CommandBuffer[] commandBuffers)
    {
        fixed (nint*            pHandle         = &this.Handle)
        fixed (VkCommandBuffer* pCommandBuffers = commandBuffers.Select(x => x.Handle).ToArray())
        {
            PInvoke.vkFreeCommandBuffers(this.Device, this, (uint)commandBuffers.Length, pCommandBuffers);
        }
    }
}
