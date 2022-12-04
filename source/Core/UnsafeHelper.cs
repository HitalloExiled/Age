using System.Text;
using Age.Extensions;

namespace Age.Core;

internal static class UnsafeUtils
{
    public static unsafe void StringToByUTF8BytesPointer(IEnumerable<string> values, byte** pEnabledExtensionNamesPointer)
    {
        var i = 0;

        foreach (var value in values)
        {
            fixed (byte* pValue = value.ToUTF8Bytes())
            {
                pEnabledExtensionNamesPointer[i++] = pValue;
            }
        }
    }
}
