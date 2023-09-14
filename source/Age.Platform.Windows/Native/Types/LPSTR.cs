using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public unsafe readonly record struct LPSTR(nint Value = default) : IDisposable
{
    public LPSTR(string? value) : this(Marshal.StringToHGlobalAnsi(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte*(LPSTR value) => (byte*)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPSTR(byte* value) => new((nint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(LPSTR value) => Marshal.PtrToStringAnsi(value.Value);
}
