using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

[DebuggerTypeProxy(typeof(NativeDictionary<,>.DebugView))]
[CollectionBuilder(typeof(Builders), nameof(Builders.NativeDictionary))]
public unsafe partial struct NativeDictionary<K, V>(int capacity, bool fixedSize) : IDisposable
where K : unmanaged, IEquatable<K>
where V : unmanaged
{
    private UnsafeDictionary* inner = UnsafeDictionary.Allocate<K, V>(capacity, fixedSize);

    public readonly V this[K key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => UnsafeDictionary.Get<K, V>(this.inner, key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => UnsafeDictionary.Set(this.inner, key, value);
    }

    public readonly int                Capacity    => UnsafeDictionary.GetCapacity(this.inner);
    public readonly int                Count       => UnsafeDictionary.GetCount(this.inner);
    public readonly KeyValueCollection Entries     => new(this);
    public readonly bool               IsCreated   => this.inner != null;
    public readonly bool               IsDisposed  => this.inner == null;
    public readonly bool               IsFixedSize => UnsafeDictionary.IsFixedSize(this.inner);
    public readonly KeyCollection      Keys        => new(this);
    public readonly ValueCollection    Values      => new(this);

    public NativeDictionary() : this(0, false)
    { }

    public NativeDictionary(int capacity) : this(capacity, false)
    { }

    public NativeDictionary(ReadOnlySpan<KeyValuePair<K, V>> keyValuePairs) : this(keyValuePairs, false)
    { }

    public NativeDictionary(ReadOnlySpan<KeyValuePair<K, V>> keyValuePairs, bool fixedSize) : this(keyValuePairs.Length, fixedSize)
    {
        foreach (var keyValuePair in keyValuePairs)
        {
            this.Add(keyValuePair.Key, keyValuePair.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Add(K key, V value) =>
        UnsafeDictionary.Add(this.inner, key, value);

    public readonly void Clear() => UnsafeDictionary.Clear(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(KeyValuePair<K, V> item) =>
        this.TryGetValue(item.Key, out var value) && EqualityComparer<V>.Default.Equals(item.Value, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool ContainsKey(K key) =>
        UnsafeDictionary.ContainsKey(this.inner, key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool ContainsValue(K key) =>
        UnsafeDictionary.ContainsValue(this.inner, key);

    public readonly void CopyTo(Span<KeyValuePair<K, V>> span, int arrayIndex) =>
        UnsafeDictionary.CopyTo(this.inner, span, arrayIndex);

    public void Dispose()
    {
        UnsafeDictionary.Free(this.inner);
        this.inner = null;
    }

    public readonly UnsafeDictionary.Enumerator<K, V> GetEnumerator() =>
        UnsafeDictionary.GetEnumerator<K, V>(this.inner);

    public readonly UnsafeDictionary.KeyEnumerator<K> GetKeyEnumerator() =>
        UnsafeDictionary.GetKeyEnumerator<K>(this.inner);

    public readonly UnsafeDictionary.ValueEnumerator<V> GetValueEnumerator() =>
        UnsafeDictionary.GetValueEnumerator<V>(this.inner);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Remove(K key) =>
        UnsafeDictionary.Remove(this.inner, key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Remove(K key, out V value) =>
        UnsafeDictionary.Remove(this.inner, key, out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Remove(KeyValuePair<K, V> item) =>
        this.Remove(item.Key);

    public override readonly string ToString() =>
        $"Count = {this.Count}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryAdd(K key, V value) =>
        UnsafeDictionary.TryAdd(this.inner, key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetValue(K key, out V value) =>
        UnsafeDictionary.TryGetValue(this.inner, key, out value);
}
