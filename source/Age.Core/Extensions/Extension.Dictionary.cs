using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    public static ref T? GetValueRefOrAddDefault<T, K>(this Dictionary<K, T> source, K key, out bool exists) where K : notnull =>
        ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out exists);

    public static ref T GetValueRefOrNullRef<T, K>(this Dictionary<K, T> source, K key) where K : notnull =>
        ref CollectionsMarshal.GetValueRefOrNullRef(source, key);
}
