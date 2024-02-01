using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public unsafe partial class DeviceMemory : DisposableNativeHandle
{
    private readonly Device device;

    internal DeviceMemory(Device device, AllocateInfo allocateInfo)
    {
        this.device = device;

        fixed (VkDeviceMemory* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkAllocateMemory(device, allocateInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkFreeMemory(this.device, this, this.device.PhysicalDevice.Instance.Allocator);

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map<T>(ulong offset, uint flags, T data) where T : unmanaged =>
        this.Map(offset, flags, [data]);

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map<T>(ulong offset, uint flags, T[] data) where T : unmanaged
    {
        var size   = sizeof(T) * data.Length;
        var buffer = stackalloc T[size];

        VulkanException.Check(PInvoke.vkMapMemory(this.device, this, offset, (ulong)size, flags, (void**)&buffer));

        PointerHelper.Copy(data, buffer);
    }

    /// <inheritdoc cref="PInvoke.vkUnmapMemory" />
    public void Unmap() =>
        PInvoke.vkUnmapMemory(this.device, this);
}
