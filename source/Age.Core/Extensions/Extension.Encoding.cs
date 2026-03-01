using System.Runtime.InteropServices;
using System.Text;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(Encoding)
    {
        public static unsafe string? GetStringFromNullTerminated(byte* value) =>
            Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(value));
    }
}
