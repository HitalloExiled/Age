using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public readonly record struct HGLRC(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HGLRC value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HGLRC(nint value) => new(value);
}
