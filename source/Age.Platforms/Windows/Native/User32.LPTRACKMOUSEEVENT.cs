using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    [DebuggerDisplay("{Value}")]
    public readonly record struct LPTRACKMOUSEEVENT(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator TRACKMOUSEEVENT*(LPTRACKMOUSEEVENT value) => (TRACKMOUSEEVENT*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator LPTRACKMOUSEEVENT(TRACKMOUSEEVENT* value) => new(new(value));
    }
}
