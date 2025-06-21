using static Age.Core.PointerHelper;

namespace ThirdParty.Vulkan;

public sealed unsafe partial class VkFence : VkDeviceResource<VkFence>
{
    public bool IsSignaled => PInvoke.vkGetFenceStatus(this.Device.Handle, this.handle) == Enums.VkResult.Success;

    internal VkFence(VkDevice device, in VkFenceCreateInfo createInfo) : base(device)
    {
        fixed (VkHandle<VkFence>*     pHandle      = &this.handle)
        fixed (VkFenceCreateInfo*     pCcreateInfo = &createInfo)
        fixed (VkAllocationCallbacks* pAllocator   = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkCreateFence(device.Handle, pCcreateInfo, NullIfDefault(pAllocator), pHandle));
        }
    }

    private static void Reset(VkHandle<VkDevice> device, VkHandle<VkFence> fence) =>
        VkException.Check(PInvoke.vkResetFences(device, 1, &fence));

    private static void Reset(VkHandle<VkDevice> device, VkHandle<VkFence>[] fences)
    {
        fixed (VkHandle<VkFence>* pHandle = fences)
        {
            VkException.Check(PInvoke.vkResetFences(device, (uint)fences.Length, pHandle));
        }
    }

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    private static void Wait(VkHandle<VkDevice> device, VkHandle<VkFence> fence, bool waitAll, ulong timeout) =>
        VkException.Check(PInvoke.vkWaitForFences(device, 1, &fence, waitAll, timeout));

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    private static void Wait(VkHandle<VkDevice> device, VkHandle<VkFence>[] fences, bool waitAll, ulong timeout)
    {
        fixed (VkHandle<VkFence>* pFences = fences)
        {
            VkException.Check(PInvoke.vkWaitForFences(device, (uint)fences.Length, pFences, waitAll, timeout));
        }
    }

    /// <inheritdoc cref="PInvoke.vkResetFences" />
    public static void Reset(VkFence[] fences)
    {
        foreach (var group in fences.GroupBy(x => x.Device))
        {
            Reset(group.Key.Handle, VkHandle.GetHandles(group.ToArray().AsSpan()));
        }
    }

    /// <inheritdoc cref="PInvoke.vkResetFences" />
    public static void Reset(VkDevice device, scoped ReadOnlySpan<VkFence> fences) =>
        Reset(device.Handle, VkHandle.GetHandles(fences));

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    public static void Wait(VkFence[] fences, bool waitAll, ulong timeout)
    {
        foreach (var group in fences.GroupBy(x => x.Device))
        {
            Wait(group.Key.Handle, VkHandle.GetHandles(group.ToArray().AsSpan()), waitAll, timeout);
        }
    }

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    public static void Wait(VkDevice device, scoped ReadOnlySpan<VkFence> fences, bool waitAll, ulong timeout) =>
        Wait(device.Handle, VkHandle.GetHandles(fences), waitAll, timeout);

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyFence(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }

    /// <inheritdoc cref="PInvoke.vkResetFences" />
    public void Reset() =>
        Reset(this.Device.Handle, this.handle);

    public void Wait() =>
        Wait(this.Device.Handle, this.handle, true, ulong.MaxValue);

    /// <inheritdoc cref="PInvoke.vkWaitForFences" />
    public void Wait(bool waitAll, ulong timeout) =>
        Wait(this.Device.Handle, this.handle, waitAll, timeout);
}
