using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Opaque handle to an image object.</para>
/// <para>Images represent multidimensional - up to 3 - arrays of data which can be used for various purposes (e.g. attachments, textures), by binding them to a graphics or compute pipeline via descriptor sets, or by directly specifying them as parameters to certain commands.</para>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkImage(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkImage(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkImage value) => value.Value;
}
