using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platforms.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public readonly record struct INT(int Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator int(INT value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator INT(int value) => new(value);
}
