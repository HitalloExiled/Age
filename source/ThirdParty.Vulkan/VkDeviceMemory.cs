using System.Runtime.InteropServices;
using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public unsafe partial class VkDeviceMemory : DeviceResource<VkDeviceMemory>
{
    internal VkDeviceMemory(VkDevice device, in VkMemoryAllocateInfo allocateInfo) : base(device)
    {
        fixed (VkHandle<VkDeviceMemory>* pHandle       = &this.handle)
        fixed (VkMemoryAllocateInfo*     pAllocateInfo = &allocateInfo)
        fixed (VkAllocationCallbacks*    pAllocator    = &this.Instance.Allocator)
        {
            VkException.Check(PInvoke.vkAllocateMemory(device.Handle, pAllocateInfo, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator), pHandle));
        }
    }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkFreeMemory(this.Device.Handle, this.handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map<T>(ulong offset, uint flags, T data) where T : unmanaged =>
        this.Map(offset, flags, [data]);

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map<T>(ulong offset, uint flags, T[] data) where T : unmanaged
    {
        var ppData = (T**)NativeMemory.Alloc((uint)(sizeof(T*) * data.Length));

        VkException.Check(PInvoke.vkMapMemory(this.Device.Handle, this.handle, offset, (uint)(sizeof(T) * data.Length), flags, (void**)ppData));

        PointerHelper.Copy(data, *ppData, (uint)data.Length);

        NativeMemory.Free(ppData);
    }

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map<T>(ulong offset, uint flags, in T* pData) where T : unmanaged
    {
        fixed (T** ppData = &pData)
        {
            PInvoke.vkMapMemory(this.Device.Handle, this.handle, offset, (ulong)Marshal.SizeOf<T>(), flags, (void**)ppData);
        }
    }

    /// <inheritdoc cref="PInvoke.vkMapMemory" />
    public void Map(ulong offset, ulong size, uint flags, in nint pData)
    {
        fixed (nint* ppData = &pData)
        {
            PInvoke.vkMapMemory(this.Device.Handle, this.handle, offset, size, flags, (void**)ppData);
        }
    }

    /// <inheritdoc cref="PInvoke.vkUnmapMemory" />
    public void Unmap() =>
        PInvoke.vkUnmapMemory(this.Device.Handle, this.handle);
}
