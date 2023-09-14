using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public unsafe readonly record struct PSTR(nint Value = default) : IDisposable
{
    public PSTR(string? value) : this(Marshal.StringToHGlobalAnsi(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte*(PSTR value) => (byte*)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator PSTR(byte* value) => new((nint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(PSTR value) => Marshal.PtrToStringAnsi(value.Value);
}
