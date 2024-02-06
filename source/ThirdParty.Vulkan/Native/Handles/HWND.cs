using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThirdParty.Vulkan.Native.Handles;

[DebuggerDisplay("{Value}")]
public record struct HWND(nint Handle)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator HWND(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nint(HWND value) => value.Handle;
}
