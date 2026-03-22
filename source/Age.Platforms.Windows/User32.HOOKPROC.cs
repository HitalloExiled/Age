using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Platforms.Windows;

internal static partial class User32
{
    [DebuggerDisplay("{Value}")]
    public record struct HOOKPROC(nint Value = default)
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate LRESULT Function(int code, WINDOW_MESSAGE wParam, LPARAM lParam);

        public HOOKPROC(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator nint(HOOKPROC value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator Function(HOOKPROC value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator HOOKPROC(Function value) => new(value);
    }
}
