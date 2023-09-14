using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public unsafe readonly record struct LPWSTR(nint Value = default) : IDisposable
{
    public LPWSTR(string? value) : this(Marshal.StringToHGlobalUni(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator char*(LPWSTR value) => (char*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPWSTR(char* value) => new(new nint(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(LPWSTR value) => Marshal.PtrToStringUni(value.Value);
}
