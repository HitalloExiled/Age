using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platforms.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public readonly record struct HMENU(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HMENU value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HMENU(nint value) => new(value);
}
