using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Age.Platform.Windows.Api.Native;

[DebuggerDisplay("{Value}")]
public readonly record struct LPCWSTR(nint Value = default) : IDisposable
{
    public LPCWSTR(string? value) : this(Marshal.StringToHGlobalUni(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator char*(LPCWSTR value) => (char*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPCWSTR(char* value) => new(new nint(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(LPCWSTR value) => Marshal.PtrToStringUni(value.Value);
}
