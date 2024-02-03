using System.Runtime.InteropServices;
using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public unsafe partial class DeviceMemory : DeviceResource
{
    internal DeviceMemory(Device device, AllocateInfo allocateInfo) : base(device)
    {
        fixed (VkDeviceMemory* pHandle = &this.Handle)
        {
            VulkanException.Check(PInvoke.vkAllocateMemory(device, allocateInfo, device.PhysicalDevice.Instance.Allocator, pHandle));
        }
    }

    protected override void OnDispose() =>
        PInvoke.vkFreeMemory(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map<T>(ulong offset, uint flags, T data) where T : unmanaged =>
        this.Map(offset, flags, [data]);

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map<T>(ulong offset, uint flags, T[] data) where T : unmanaged
    {
        var ppData = (T**)NativeMemory.Alloc((uint)(sizeof(T*) * data.Length));

        VulkanException.Check(PInvoke.vkMapMemory(this.Device, this, offset, (uint)(sizeof(T) * data.Length), flags, (void**)ppData));

        PointerHelper.Copy(data, *ppData, (uint)data.Length);

        NativeMemory.Free(ppData);
    }

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map<T>(ulong offset, uint flags, T** ppData) where T : unmanaged =>
        PInvoke.vkMapMemory(this.Device, this, offset, (ulong)Marshal.SizeOf<T>(), flags, (void**)ppData);

    /// <inheritdoc cref="PInvoke.vkUnmapMemory" />
    public void Unmap() =>
        PInvoke.vkUnmapMemory(this.Device, this);
}
