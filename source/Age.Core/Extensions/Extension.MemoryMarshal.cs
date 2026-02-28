using System.Runtime.InteropServices;
using System.Text;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(MemoryMarshal)
    {
        public unsafe static byte* CreateUTF8StringBuffer(string? value)
        {
            if (value == null)
            {
                return null;
            }

            var bytesCount = Encoding.UTF8.GetByteCount(value);

            var pData = (byte*)NativeMemory.Alloc((nuint)bytesCount + 1);

            var written = Encoding.UTF8.GetBytes(value, new Span<byte>(pData, bytesCount));

            pData[written] = 0;

            return pData;
        }
    }
}
