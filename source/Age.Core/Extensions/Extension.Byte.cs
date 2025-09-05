
using System.Runtime.CompilerServices;
using System.Text;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(byte[] source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ConvertToString() =>
            Encoding.Default.GetString(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ConvertToString(Encoding encoding) =>
            encoding.GetString(source);
    }
}
