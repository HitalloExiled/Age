using System.Text;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(Array)
    {
        public unsafe static string[] ToUTF8StringArray(byte** ppSource, uint length)
        {
            var result = new string[length];

            for (var i = 0; i < length; i++)
            {
                result[i] = Encoding.GetStringFromNullTerminated(ppSource[i])!;
            }

            return result;
        }
    }

    extension<T>(T[] source)
    {
        public void TimSort(Func<T, T, int>? comparer = null) =>
            source.AsSpan().TimSort(comparer);
    }
}
