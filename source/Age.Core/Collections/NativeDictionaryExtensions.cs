namespace Age.Core.Collections;

public static class NativeDictionaryExtensions
{
    extension<K, V>(NativeDictionary<K, V> dictionary)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged, IEquatable<V>
    {
        public unsafe bool ContainsValue(V value) =>
            UnsafeDictionary.ContainsValue(dictionary.GetUnsafeDictionary(), value);
    }
}
