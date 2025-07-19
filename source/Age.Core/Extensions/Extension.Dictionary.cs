using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension<T, K>(Dictionary<K, T> source) where K : notnull
    {
        public ref T? GetValueRefOrAddDefault(K key, out bool exists) =>
            ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out exists);

        public ref T GetValueRefOrNullRef(K key) =>
            ref CollectionsMarshal.GetValueRefOrNullRef(source, key);
    }
}
