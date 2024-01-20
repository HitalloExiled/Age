using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Opaque handle to a descriptor set layout object.</para>
/// <para>A descriptor set layout object is defined by an array of zero or more descriptor bindings. Each individual descriptor binding is specified by a descriptor type, a count (array size) of the number of descriptors in the binding, a set of shader stages that can access the binding, and (if using immutable samplers) an array of sampler descriptors.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkDescriptorSetLayout(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDescriptorSetLayout(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkDescriptorSetLayout value) => value.Value;
}
