using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core.Collections;

internal unsafe partial struct UnsafeHashCollection
{
    public Entry** Buckets;
    public Entry*  FreeHead;

    public UnsafeBuffer Entries;

    public int FreeCount;
    public int KeyOffset;
    public int UsedCount;

    public static void Clear(UnsafeHashCollection* collection)
    {
        collection->FreeHead  = null;
        collection->FreeCount = 0;
        collection->UsedCount = 0;

        var length = collection->Entries.Length;

        NativeMemory.Clear(collection->Buckets, (nuint)(length * sizeof(Entry**)));
        UnsafeBuffer.Clear(&collection->Entries);
    }

    private static void Expand(UnsafeHashCollection* collection)
    {
        Debug.Assert(collection->Entries.Dynamic == 1);

        var capacity = Math.GetPrime(collection->Entries.Length + 1);

        Debug.Assert(capacity >= collection->Entries.Length);

        var newBuckets = (Entry**)NativeMemory.AllocZeroed((nuint)(capacity * sizeof(Entry**)), (nuint)sizeof(Entry**));

        UnsafeBuffer.ResizeDynamic(&collection->Entries, capacity, true);

        collection->FreeHead  = null;
        collection->FreeCount = 0;

        for (var i = collection->Entries.Length - 1; i >= 0; --i)
        {
            var entry = (Entry*)((byte*)collection->Entries.Pointer + (i * collection->Entries.Stride));

            if (entry->State == EntryState.Used)
            {
                var bucketHash = entry->Hash % capacity;

                entry->Next = newBuckets[bucketHash];

                newBuckets[bucketHash] = entry;
            }
            else if (entry->State == EntryState.Free)
            {
                entry->Next = collection->FreeHead;

                collection->FreeHead = entry;
                collection->FreeCount++;
            }
        }

        NativeMemory.Free(collection->Buckets);

        collection->Buckets = newBuckets;
    }

    public static void Free(UnsafeHashCollection* collection)
    {
        if (collection == null)
        {
            return;
        }

        Debug.Assert(collection->Entries.Dynamic == 1);

        NativeMemory.Free(collection->Buckets);
        NativeMemory.Free(collection->Entries.Pointer);

        *collection = default;
    }

    public static Entry* Find<T>(UnsafeHashCollection* collection, T key, int valueHash)
    where T : unmanaged, IEquatable<T>
    {
        var bucketHead = collection->Buckets[valueHash % collection->Entries.Length];

        while (bucketHead != null)
        {
            if (bucketHead->Hash == valueHash && key.Equals(*(T*)((byte*)bucketHead + collection->KeyOffset)))
            {
                return bucketHead;
            }
            else
            {
                bucketHead = bucketHead->Next;
            }
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetKey<T>(UnsafeHashCollection* collection, Entry* entry) where T : unmanaged =>
        *(T*)((byte*)entry + collection->KeyOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Entry* GetEntry(UnsafeHashCollection* collection, int index) =>
        collection->Entries.Element<Entry>(index);

    public static Entry* Insert<T>(UnsafeHashCollection* collection, T key, int valueHash) where T : unmanaged
    {
        Entry* entry;

        if (collection->FreeHead != null)
        {
            Debug.Assert(collection->FreeCount > 0);

            entry = collection->FreeHead;

            collection->FreeHead = entry->Next;
            collection->FreeCount--;

            Debug.Assert(entry->State == EntryState.Free, ExceptionMessages.CONCURRENT_OPERATION_ARE_NOT_SUPPORTED);
        }
        else
        {
            if (collection->UsedCount == collection->Entries.Length)
            {
                if (collection->Entries.Dynamic == 0)
                {
                    throw new InvalidOperationException("Fixed size collection is full.");
                }

                Expand(collection);
            }

            entry = collection->Entries.Element<Entry>(collection->UsedCount);

            collection->UsedCount++;

            Debug.Assert(entry->State == EntryState.None, ExceptionMessages.CONCURRENT_OPERATION_ARE_NOT_SUPPORTED);
        }

        var bucketHash = valueHash % collection->Entries.Length;

        entry->Hash  = valueHash;
        entry->Next  = collection->Buckets[bucketHash];
        entry->State = EntryState.Used;

        *(T*)((byte*)entry + collection->KeyOffset) = key;

        collection->Buckets[bucketHash] = entry;

        return entry;
    }

    public static bool Remove<T>(UnsafeHashCollection* collection, T key, int valueHash) where T : unmanaged, IEquatable<T>
    {
        var bucketHash = valueHash % collection->Entries.Length;
        var bucketHead = collection->Buckets[valueHash % collection->Entries.Length];
        var bucketPrev = default(Entry*);

        while (bucketHead != null)
        {
            if (bucketHead->Hash == valueHash && key.Equals(*(T*)((byte*)bucketHead + collection->KeyOffset)))
            {
                if (bucketPrev == null)
                {
                    collection->Buckets[bucketHash] = bucketHead->Next;
                }
                else
                {
                    bucketPrev->Next = bucketHead->Next;
                }

                Debug.Assert(bucketHead->State == EntryState.Used);

                bucketHead->Next  = collection->FreeHead;
                bucketHead->State = EntryState.Free;

                collection->FreeHead = bucketHead;
                collection->FreeCount++;

                return true;
            }
            else
            {
                bucketPrev = bucketHead;
                bucketHead = bucketHead->Next;
            }
        }

        return false;
    }
}
