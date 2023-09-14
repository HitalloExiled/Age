
using System.Runtime.CompilerServices;
using System.Text;

namespace Age.Core.Extensions;

public static class ByteExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ConvertToString(this byte[] source) =>
        Encoding.Default.GetString(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ConvertToString(this byte[] source, Encoding encoding) =>
        encoding.GetString(source);
}
