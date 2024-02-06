using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThirdParty.Vulkan.Native.Handles;

[DebuggerDisplay("{Value}")]
public record struct HINSTANCE(nint Handle)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator HINSTANCE(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nint(HINSTANCE value) => value.Handle;
}
