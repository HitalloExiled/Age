namespace Age.Core.Extensions;

public static partial class Extension
{
    extension<T>(T[] source)
    {
        public void TimSort(Func<T, T, int>? comparer = null) =>
            source.AsSpan().TimSort(comparer);
    }
}
