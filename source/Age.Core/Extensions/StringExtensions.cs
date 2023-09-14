using System.Runtime.CompilerServices;
using System.Text;

namespace Age.Core.Extensions;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte[] ToBytes(this string value) =>
        Encoding.Default.GetBytes(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte[] ToUTF8Bytes(this string value) =>
        Encoding.UTF8.GetBytes(value);
}
