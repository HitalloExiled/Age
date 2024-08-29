using static Age.Core.Interop.PointerHelper;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipeline.html">VkPipeline</see>
/// </summary>
public unsafe abstract class VkPipeline : VkDeviceResource<VkPipeline>
{
    internal VkPipeline(VkDevice device) : base(device) { }
    internal VkPipeline(VkHandle<VkPipeline> handle, VkDevice device) : base(handle, device) { }

    protected override void Disposed()
    {
        fixed (VkAllocationCallbacks* pAllocator = &this.Instance.Allocator)
        {
            PInvoke.vkDestroyPipeline(this.Device.Handle, this.handle, NullIfDefault(pAllocator));
        }
    }
}
