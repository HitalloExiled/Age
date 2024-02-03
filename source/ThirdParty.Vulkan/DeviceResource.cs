namespace ThirdParty.Vulkan;

public abstract class DeviceResource : DisposableNativeHandle
{
    internal Device Device { get; }

    internal DeviceResource(Device device) : base() =>
        this.Device = device;

    internal DeviceResource(nint handle, Device device) : base(handle) =>
        this.Device = device;
}
