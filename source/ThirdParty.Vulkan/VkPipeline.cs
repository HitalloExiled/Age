using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipeline.html">VkPipeline</see>
/// </summary>
public unsafe abstract class VkPipeline : DeviceResource<VkPipeline>
{
    internal VkPipeline(VkDevice device) : base(device) { }
    internal VkPipeline(VkHandle<VkPipeline> handle, VkDevice device) : base(handle, device) { }

    protected override void OnDispose()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyPipeline(this.Device.Handle, this.Handle, PointerHelper.NullIfDefault(this.Instance.Allocator, pAllocator));
        }
    }
}
