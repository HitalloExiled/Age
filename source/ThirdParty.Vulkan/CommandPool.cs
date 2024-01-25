using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

public unsafe partial class CommandPool : DisposableNativeHandle
{
    private readonly AllocationCallbacks? allocator;
    private readonly Device               device;

    public CommandPool(Device device, CreateInfo createInfo, AllocationCallbacks? allocator)
    {
        this.device    = device;
        this.allocator = allocator;

        fixed (VkCommandPool* pValue = &this.Handle)
        {
            PInvoke.vkCreateCommandPool(device, createInfo, allocator, pValue);
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkDestroyCommandPool(this.device, this.Handle, this.allocator);

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
            VulkanException.Check(PInvoke.vkAllocateCommandBuffers(this.device, allocateInfo, pBuffer));
        }

        var commands = new CommandBuffer[count];

        for (var i = 0; i < count; i++)
        {
            commands[i] = new(buffer[i], this.device, this);
        }

        return commands;
    }

    public void FreeCommandBuffers(params CommandBuffer[] commandBuffers)
    {
        fixed (nint*            pHandle         = &this.Handle)
        fixed (VkCommandBuffer* pCommandBuffers = commandBuffers.Select(x => x.Handle).ToArray())
        {
            PInvoke.vkFreeCommandBuffers(this.device, this, (uint)commandBuffers.Length, pCommandBuffers);
        }
    }
}
