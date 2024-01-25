using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class PhysicalDevice
{

    /// <inheritdoc cref="VkPhysicalDeviceSparseProperties" />
    public record SparseProperties : NativeReference<VkPhysicalDeviceSparseProperties>
    {
        internal SparseProperties(in VkPhysicalDeviceSparseProperties sparseProperties) : base(sparseProperties) { }
    }
}
