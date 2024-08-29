using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platforms.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public unsafe readonly record struct PUINT(nuint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator UINT*(PUINT value) => (UINT*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator PUINT(UINT* value) => new(new(value));
}
