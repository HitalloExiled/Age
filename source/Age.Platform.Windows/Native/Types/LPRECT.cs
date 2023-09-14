using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public unsafe readonly record struct LPRECT(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator RECT*(LPRECT value) => (RECT*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPRECT(RECT* value) => new((nint)value);
}
