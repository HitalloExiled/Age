using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public unsafe readonly record struct LPCSTR(nint Value = default) : IDisposable
{
    public LPCSTR(string? value) : this(Marshal.StringToHGlobalAnsi(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte*(LPCSTR value) => (byte*)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPCSTR(byte* value) => new((nint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(LPCSTR value) => Marshal.PtrToStringAnsi(value.Value);
}
