using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Opaque handle to a buffer view object.</para>
/// <para>A buffer view represents a contiguous range of a buffer and a specific format to be used to interpret the data. Buffer views are used to enable shaders to access buffer contents using <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#textures">image operations</see>. In order to create a valid buffer view, the buffer must have been created with at least one of the following usage flags:</para>
/// <list type="bullet">
/// <item>VK_BUFFER_USAGE_UNIFORM_TEXEL_BUFFER_BIT</item>
/// <item>VK_BUFFER_USAGE_STORAGE_TEXEL_BUFFER_BIT</item>
/// </list>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkBufferView(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkBufferView(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkBufferView value) => value.Value;
}
