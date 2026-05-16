using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeDictionary
{
    private UnsafeHashCollection collection;

#if DEBUG
    private nint typeHandleKey;
    private nint typeHandleValue;
#endif

    private int valueOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertKeySafeGuards<K>(UnsafeDictionary* dictionary)
    {
#if DEBUG
        Debug.Assert(dictionary != null);
        Debug.Assert(typeof(K).TypeHandle.Value == dictionary->typeHandleKey);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertValueSafeGuards<V>(UnsafeDictionary* dictionary)
    {
#if DEBUG
        Debug.Assert(dictionary != null);
        Debug.Assert(typeof(V).TypeHandle.Value == dictionary->typeHandleKey);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSafeGuards<K, V>(UnsafeDictionary* dictionary)
    {
#if DEBUG
        Debug.Assert(dictionary != null);
        Debug.Assert(typeof(K).TypeHandle.Value == dictionary->typeHandleKey);
        Debug.Assert(typeof(V).TypeHandle.Value == dictionary->typeHandleValue);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static V* GetValue<V>(int offset, UnsafeHashCollection.Entry* pair) where V : unmanaged, allows ref struct =>
        (V*)((byte*)pair + offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetSafeGuards<K, V>(UnsafeDictionary* dictionary)
    {
        Debug.Assert(dictionary != null);
#if DEBUG
        dictionary->typeHandleKey   = typeof(K).TypeHandle.Value;
        dictionary->typeHandleValue = typeof(V).TypeHandle.Value;
#endif
    }

    public static void Add<K, V>(UnsafeDictionary* dictionary, K key, V value)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged => TryInsert(dictionary, key, value, MapInsertionBehaviour.ThrowIfExists);

    public static void AddOrGet<K, V>(UnsafeDictionary* dictionary, K key, ref V value)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged
    {
        AssertSafeGuards<K, V>(dictionary);

        var hash = key.GetHashCode();

        var entry = UnsafeHashCollection.Find(&dictionary->collection, key, hash);

        if (entry == null)
        {
            entry = UnsafeHashCollection.Insert(&dictionary->collection, key, hash);

            *GetValue<V>(dictionary->valueOffset, entry) = value;
        }
        else
        {
            value = *GetValue<V>(dictionary->valueOffset, entry);
        }
    }

    public static UnsafeDictionary* Allocate<K, V>(int capacity, bool fixedSize = false)
    where K : unmanaged
    where V : unmanaged
    {
        var keyStride   = sizeof(K);
        var valStride   = sizeof(V);
        var entryStride = sizeof(UnsafeHashCollection.Entry);

        capacity = Math.GetPrime(capacity);

        var keyAlignment = Marshal.GetAlignment(keyStride);
        var valAlignment = Marshal.GetAlignment(valStride);

        var alignment = Math.Max(UnsafeHashCollection.Entry.ALIGNMENT, Math.Max(keyAlignment, valAlignment));

        keyStride   = Marshal.RoundToAlignment(keyStride, alignment);
        valStride   = Marshal.RoundToAlignment(valStride, alignment);
        entryStride = Marshal.RoundToAlignment(sizeof(UnsafeHashCollection.Entry), alignment);

        UnsafeDictionary* dictionary;

        if (fixedSize)
        {
            var sizeOfHeader        = Marshal.RoundToAlignment(sizeof(UnsafeDictionary), alignment);
            var sizeOfBucketsBuffer = Marshal.RoundToAlignment(sizeof(UnsafeHashCollection.Entry**) * capacity, alignment);
            var sizeofEntriesBuffer = (entryStride + keyStride + valStride) * capacity;

            var ptr = NativeMemory.AlignedAllocZeroed((nuint)(sizeOfHeader + sizeOfBucketsBuffer + sizeofEntriesBuffer), (nuint)alignment);

            dictionary = (UnsafeDictionary*)ptr;

            dictionary->collection.Buckets = (UnsafeHashCollection.Entry**)((byte*)ptr + sizeOfHeader);

            UnsafeBuffer.InitFixed(&dictionary->collection.Entries, (byte*)ptr + (sizeOfHeader + sizeOfBucketsBuffer), capacity, entryStride + keyStride + valStride);
        }
        else
        {
            dictionary = NativeMemory.AllocZeroed<UnsafeDictionary>();

            dictionary->collection.Buckets = (UnsafeHashCollection.Entry**)NativeMemory.AllocZeroed((nuint)(sizeof(UnsafeHashCollection.Entry**) * capacity), (nuint)sizeof(UnsafeHashCollection.Entry**));

            UnsafeBuffer.InitDynamic(&dictionary->collection.Entries, capacity, entryStride + keyStride + valStride);
        }

        dictionary->collection.UsedCount = 0;
        dictionary->collection.KeyOffset = entryStride;
        dictionary->valueOffset          = entryStride + keyStride;

        SetSafeGuards<K, V>(dictionary);

        return dictionary;
    }

    public static void Clear(UnsafeDictionary* dictionary)
    {
        Debug.Assert(dictionary != null);

        UnsafeHashCollection.Clear(&dictionary->collection);
    }

    public static bool ContainsKey<K>(UnsafeDictionary* dictionary, K key) where K : unmanaged, IEquatable<K>
    {
        AssertKeySafeGuards<K>(dictionary);

        return UnsafeHashCollection.Find(&dictionary->collection, key, key.GetHashCode()) != null;
    }

    public static bool ContainsValue<V>(UnsafeDictionary* dictionary, V value)
    where V : unmanaged, IEquatable<V>
    {
        var iterator = new ValueEnumerator<V>(dictionary);

        while (iterator.MoveNext())
        {
            if (value.Equals(iterator.Current))
            {
                return true;
            }
        }

        return false;
    }

    public static void CopyTo<K, V>(UnsafeDictionary* dictionary, Span<KeyValuePair<K, V>> destination, int destinationIndex)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged
    {
        if (GetCount(dictionary) + (uint)destinationIndex > destination.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(destination), "Destination is not long enough to copy all the items in the collection. Check array index and length.");
        }

        AssertSafeGuards<K, V>(dictionary);
        Debug.Assert(destination.Length >= GetCount(dictionary) + destinationIndex);

        var enumerator = GetEnumerator<K, V>(dictionary);

        for (var i = 0; enumerator.MoveNext(); i++)
        {
            destination[destinationIndex + i] = enumerator.Current;
        }
    }

    public static void Free(UnsafeDictionary* set)
    {
        if (set == null)
        {
            return;
        }

        if (set->collection.Entries.Dynamic == 1)
        {
            UnsafeHashCollection.Free(&set->collection);
        }

        *set = default;

        NativeMemory.Free(set);
    }

    public static V Get<K, V>(UnsafeDictionary* dictionary, K key)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged
    {
        AssertSafeGuards<K, V>(dictionary);

        var entry = UnsafeHashCollection.Find(&dictionary->collection, key, key.GetHashCode());

        return entry == null ? throw new ArgumentException($"Value not found for key {key}") : *GetValue<V>(dictionary->valueOffset, entry);
    }

    public static int GetCapacity(UnsafeDictionary* dictionary)
    {
        Debug.Assert(dictionary != null);

        return dictionary->collection.Entries.Length;
    }

    public static int GetCount(UnsafeDictionary* dictionary)
    {
        Debug.Assert(dictionary != null);

        return dictionary->collection.UsedCount - dictionary->collection.FreeCount;
    }

    public static Enumerator<K, V> GetEnumerator<K, V>(UnsafeDictionary* dictionary)
    where K : unmanaged
    where V : unmanaged
    {
        AssertSafeGuards<K, V>(dictionary);

        return new Enumerator<K, V>(dictionary);
    }

    public static KeyEnumerator<K> GetKeyEnumerator<K>(UnsafeDictionary* dictionary)
    where K : unmanaged, IEquatable<K>
    {
       AssertKeySafeGuards<K>(dictionary);

        return new KeyEnumerator<K>(dictionary);
    }

    public static ValueEnumerator<V> GetValueEnumerator<V>(UnsafeDictionary* dictionary)
    where V : unmanaged
    {
        AssertValueSafeGuards<V>(dictionary);

        return new ValueEnumerator<V>(dictionary);
    }

    public static bool IsFixedSize(UnsafeDictionary* dictionary)
    {
        Debug.Assert(dictionary != null);

        return dictionary->collection.Entries.Dynamic == 0;
    }

    public static bool Remove<K>(UnsafeDictionary* dictionary, K key) where K : unmanaged, IEquatable<K>
    {
        AssertKeySafeGuards<K>(dictionary);

        return UnsafeHashCollection.Remove(&dictionary->collection, key, key.GetHashCode());
    }

    public static bool Remove<K, V>(UnsafeDictionary* dictionary, K key, out V value)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged
    {
        AssertSafeGuards<K, V>(dictionary);

        if (UnsafeHashCollection.Remove(&dictionary->collection, key, key.GetHashCode()))
        {
            value = *GetValue<V>(dictionary->valueOffset, dictionary->collection.FreeHead);

            return true;
        }

        value = default;

        return false;
    }

    public static void Set<K, V>(UnsafeDictionary* dictionary, K key, V value)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged =>
        TryInsert(dictionary, key, value, MapInsertionBehaviour.Overwrite);

    public static bool TryAdd<K, V>(UnsafeDictionary* dictionary, K key, V value)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged =>
        TryInsert(dictionary, key, value, MapInsertionBehaviour.None);

    public static bool TryGetValue<K, V>(UnsafeDictionary* dictionary, K key, out V val)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged
    {
        AssertSafeGuards<K, V>(dictionary);

        var entry = UnsafeHashCollection.Find(&dictionary->collection, key, key.GetHashCode());

        if (entry != null)
        {
            val = *GetValue<V>(dictionary->valueOffset, entry);
            return true;
        }

        val = default;
        return false;
    }

    private static bool TryInsert<K, V>(UnsafeDictionary* dictionary, K key, V value, MapInsertionBehaviour behaviour)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged
    {
        AssertSafeGuards<K, V>(dictionary);

        var hash  = key.GetHashCode();
        var entry = UnsafeHashCollection.Find(&dictionary->collection, key, hash);

        if (entry != null)
        {
            if (behaviour == MapInsertionBehaviour.Overwrite)
            {
                *GetValue<V>(dictionary->valueOffset, entry) = value;

                return true;
            }

            return behaviour == MapInsertionBehaviour.ThrowIfExists
                ? throw new ArgumentException($"An item with the same key has already been added. Key: {key}")
                : false;
        }
        else
        {
            entry = UnsafeHashCollection.Insert(&dictionary->collection, key, hash);

            *GetValue<V>(dictionary->valueOffset, entry) = value;

            return true;
        }
    }
}

internal enum MapInsertionBehaviour : byte
{
    None          = 0,
    Overwrite     = 1,
    ThrowIfExists = 2
}
