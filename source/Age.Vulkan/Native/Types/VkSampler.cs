using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Opaque handle to a sampler object.</para>
/// <para><see cref="VkSampler"/> objects represent the state of an image sampler which is used by the implementation to read image data and apply filtering and other transformations for the shader.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkSampler(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkSampler(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkSampler value) => value.Value;
}
