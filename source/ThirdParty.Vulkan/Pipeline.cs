namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipeline.html">VkPipeline</see>
/// </summary>
public unsafe abstract class Pipeline : DeviceResource
{
    internal Pipeline(Device device) : base(device) { }
    internal Pipeline(nint handle, Device device) : base(handle, device) { }

    protected override void OnDispose() =>
        PInvoke.vkDestroyPipeline(this.Device, this, this.Device.PhysicalDevice.Instance.Allocator);
}
