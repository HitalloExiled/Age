using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Platform.Windows.Native.Types;

namespace Age.Platform.Windows.Native;

internal static partial class User32
{
    [DebuggerDisplay("{Value}")]
    public record struct MONITORENUMPROC(nint Value = default)
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate BOOL Function(HMONITOR hMonitor, HDC hdc, LPRECT lpRect, LPARAM lParam);

        public MONITORENUMPROC(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator nint(MONITORENUMPROC value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator Function(MONITORENUMPROC value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator MONITORENUMPROC(Function value) => new(value);
    }
}
