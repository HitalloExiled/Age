using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeHashSet
{
    private UnsafeHashCollection collection;

#if DEBUG
    private nint typeHandle;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssertSafeGuards<T>(UnsafeHashSet* set)
    {
#if DEBUG
        Debug.Assert(set != null);
        Debug.Assert(typeof(T).TypeHandle.Value == set->typeHandle);
#endif
    }

    public static bool Add<T>(UnsafeHashSet* set, T key)
    where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(set);

        var hash  = key.GetHashCode();
        var entry = UnsafeHashCollection.Find(&set->collection, key, hash);

        if (entry != null)
        {
            return false;
        }

        UnsafeHashCollection.Insert(&set->collection, key, hash);

        return true;
    }

    public static UnsafeHashSet* Allocate<T>(int capacity, bool fixedSize = false)
    where T : unmanaged, IEquatable<T>
    {
        var keyStride   = sizeof(T);
        var entryStride = sizeof(UnsafeHashCollection.Entry);

        capacity = Math.GetPrime(capacity);

        var keyAlignment = Marshal.GetAlignment(keyStride);
        var alignment    = Math.Max(UnsafeHashCollection.Entry.ALIGNMENT, keyAlignment);

        keyStride   = Marshal.RoundToAlignment(keyStride, alignment);
        entryStride = Marshal.RoundToAlignment(sizeof(UnsafeHashCollection.Entry), alignment);

        UnsafeHashSet* set;

        if (fixedSize)
        {
            var sizeOfHeader        = Marshal.RoundToAlignment(sizeof(UnsafeHashSet), alignment);
            var sizeOfBucketsBuffer = Marshal.RoundToAlignment(sizeof(UnsafeHashCollection.Entry**) * capacity, alignment);
            var sizeOfEntriesBuffer = (entryStride + keyStride) * capacity;

            var ptr = NativeMemory.AlignedAllocZeroed((nuint)(sizeOfHeader + sizeOfBucketsBuffer + sizeOfEntriesBuffer), (nuint)alignment);

            set = (UnsafeHashSet*)ptr;

            set->collection.Buckets = (UnsafeHashCollection.Entry**)((byte*)ptr + sizeOfHeader);

            UnsafeBuffer.InitFixed(&set->collection.Entries, (byte*)ptr + (sizeOfHeader + sizeOfBucketsBuffer), capacity, entryStride + keyStride);
        }
        else
        {
            set = NativeMemory.AllocZeroed<UnsafeHashSet>();

            set->collection.Buckets = (UnsafeHashCollection.Entry**)NativeMemory.AllocZeroed((nuint)(sizeof(UnsafeHashCollection.Entry**) * capacity), (nuint)sizeof(UnsafeHashCollection.Entry**));

            UnsafeBuffer.InitDynamic(&set->collection.Entries, capacity, entryStride + keyStride);
        }

        set->collection.UsedCount = 0;
        set->collection.KeyOffset = entryStride;

#if DEBUG
        set->typeHandle = typeof(T).TypeHandle.Value;
#endif

        return set;
    }

    public static void Clear(UnsafeHashSet* set)
    {
        Debug.Assert(set != null);

        UnsafeHashCollection.Clear(&set->collection);
    }

    public static bool Contains<T>(UnsafeHashSet* set, T key)
    where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(set);

        return UnsafeHashCollection.Find(&set->collection, key, key.GetHashCode()) != null;
    }

    public static void CopyTo<T>(UnsafeHashSet* set, Span<T> destination)
    where T : unmanaged
    {
        AssertSafeGuards<T>(set);

        var enumerator = GetEnumerator<T>(set);
        var dest       = destination;

        for (var i = 0; enumerator.MoveNext(); i++)
        {
            dest[i] = enumerator.Current;
        }
    }

    public static void Free(UnsafeHashSet* set)
    {
        if (set == null)
        {
            return;
        }

        if (set->collection.Entries.Dynamic == 1)
        {
            UnsafeHashCollection.Free(&set->collection);

            *set = default;

            NativeMemory.Free(set);
        }
        else
        {
            *set = default;

            NativeMemory.AlignedFree(set);
        }
    }

    public static int GetCapacity(UnsafeHashSet* set)
    {
        Debug.Assert(set != null);

        return set->collection.Entries.Length;
    }

    public static int GetCount(UnsafeHashSet* set)
    {
        Debug.Assert(set != null);

        return set->collection.UsedCount - set->collection.FreeCount;
    }

    public static Enumerator<T> GetEnumerator<T>(UnsafeHashSet* set)
    where T : unmanaged
    {
        AssertSafeGuards<T>(set);

        return new Enumerator<T>(set);
    }

    public static bool IsFixedSize(UnsafeHashSet* set)
    {
        Debug.Assert(set != null);

        return set->collection.Entries.Dynamic == 0;
    }

    public static bool Remove<T>(UnsafeHashSet* set, T key)
    where T : unmanaged, IEquatable<T>
    {
        AssertSafeGuards<T>(set);

        return UnsafeHashCollection.Remove(&set->collection, key, key.GetHashCode());
    }
}
