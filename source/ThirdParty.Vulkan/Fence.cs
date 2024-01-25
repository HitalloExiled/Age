namespace ThirdParty.Vulkan;

public unsafe partial class Fence : DisposableNativeHandle
{
    private readonly AllocationCallbacks? allocator;
    private readonly Device               device;

    internal Fence(Device device, CreateInfo createInfo, AllocationCallbacks? allocator)
    {
        fixed (VkFence* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateFence(device, createInfo, allocator, pHandle));
        }

        this.device    = device;
        this.allocator = allocator;
    }

    private static void Wait(VkDevice device, VkFence[] fences, bool waitAll, ulong timeout)
    {
        fixed (VkFence* pHandle = fences)
        {
            VulkanException.Check(PInvoke.vkWaitForFences(device, (uint)fences.Length, pHandle, waitAll, timeout));
        }
    }

    public static void Wait(Fence[] fences, bool waitAll, ulong timeout)
    {
        foreach (var group in fences.GroupBy(x => x.device))
        {
            Wait(group.Key, group.Select(x => x.Handle).ToArray(), waitAll, timeout);
        }
    }

    public static void Wait(Device device, Fence[] fences, bool waitAll, ulong timeout) =>
        Wait(device, fences.Select(x => x.Handle).ToArray(), waitAll, timeout);

    protected override void OnDispose() =>
        PInvoke.vkDestroyFence(this.device, this, this.allocator);

    public void Reset() => throw new NotImplementedException();

    public void Wait(bool waitAll, ulong timeout) =>
        Wait(this.device, [this.Handle], waitAll, timeout);
}
