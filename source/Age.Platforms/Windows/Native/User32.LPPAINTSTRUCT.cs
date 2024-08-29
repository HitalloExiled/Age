using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    [DebuggerDisplay("{Value}")]
    public readonly record struct LPPAINTSTRUCT(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator PAINTSTRUCT*(LPPAINTSTRUCT value) => (PAINTSTRUCT*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator LPPAINTSTRUCT(PAINTSTRUCT* value) => new(new(value));
    }
}
