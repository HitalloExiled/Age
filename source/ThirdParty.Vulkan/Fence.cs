using ThirdParty.Vulkan.TypeExtensions;

namespace ThirdParty.Vulkan;

public unsafe partial class Fence : DeviceResource
{
    internal Fence(Device device, CreateInfo createInfo) : base(device)
    {
        fixed (VkFence* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkCreateFence(device, createInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    private static void Reset(VkDevice device, VkFence fence) =>
        VulkanException.Check(PInvoke.vkResetFences(device, 1, &fence));

    private static void Reset(VkDevice device, VkFence[] fences)
    {
        fixed (VkFence* pHandle = fences)
        {
            VulkanException.Check(PInvoke.vkResetFences(device, (uint)fences.Length, pHandle));
        }
    }

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    private static void Wait(VkDevice device, VkFence fence, bool waitAll, ulong timeout) =>
        VulkanException.Check(PInvoke.vkWaitForFences(device, 1, &fence, waitAll, timeout));


    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    private static void Wait(VkDevice device, VkFence[] fences, bool waitAll, ulong timeout)
    {
        fixed (VkFence* pFences = fences)
        {
            VulkanException.Check(PInvoke.vkWaitForFences(device, (uint)fences.Length, pFences, waitAll, timeout));
        }
    }

    /// <inheritdoc cref="PInvoke.vkResetFences" />
    public static void Reset(Fence[] fences)
    {
        foreach (var group in fences.GroupBy(x => x.Device))
        {
            Reset(group.Key, group.ToHandlers());
        }
    }

    /// <inheritdoc cref="PInvoke.vkResetFences" />
    public static void Reset(Device device, Fence[] fences) =>
        Reset(device, fences.ToHandlers());

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    public static void Wait(Fence[] fences, bool waitAll, ulong timeout)
    {
        foreach (var group in fences.GroupBy(x => x.Device))
        {
            Wait(group.Key, group.ToHandlers(), waitAll, timeout);
        }
    }

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    public static void Wait(Device device, Fence[] fences, bool waitAll, ulong timeout) =>
        Wait(device, fences.ToHandlers(), waitAll, timeout);

    protected override void OnDispose() =>
        PInvoke.vkDestroyFence(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);

    /// <inheritdoc cref="PInvoke.vkResetFences" />
    public void Reset() =>
        Reset(this.Device, this);

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    public void Wait(bool waitAll, ulong timeout) =>
        Wait(this.Device, this, waitAll, timeout);
}
