using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Platforms.Linux.Wayland;

internal static class Helper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static byte* Ustr(string? value) =>
        MemoryMarshal.CreateUTF8StringBuffer(value);
}
