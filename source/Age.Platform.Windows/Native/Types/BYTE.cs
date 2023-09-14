using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public readonly record struct BYTE(byte Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte(BYTE value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator BYTE(byte value) => new(value);
}
