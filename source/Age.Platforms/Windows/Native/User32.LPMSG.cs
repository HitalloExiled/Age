using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    [DebuggerDisplay("{Value}")]
    public readonly record struct LPMSG(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator MSG*(LPMSG value) => (MSG*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator LPMSG(MSG* value) => new(new(value));
    }
}
