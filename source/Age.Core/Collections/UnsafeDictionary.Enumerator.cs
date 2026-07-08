using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeDictionary
{
    public struct Enumerator<K, V>(UnsafeDictionary* map)
    where K : unmanaged
    where V : unmanaged
    {
        private UnsafeHashCollection.Enumerator iterator = new(&map->collection);
        private readonly int keyOffset = map->collection.KeyOffset;
        private readonly int valueOffset = map->valueOffset;

        public readonly KeyValuePair<K, V> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(this.CurrentKey, this.CurrentValue);
        }

        public readonly K CurrentKey
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(this.iterator.Current != null);

                return *(K*)((byte*)this.iterator.Current + this.keyOffset);
            }
        }

        public readonly V CurrentValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(this.iterator.Current != null);

                return *(V*)((byte*)this.iterator.Current + this.valueOffset);
            }
        }

        public bool MoveNext() => this.iterator.MoveNext();

        public void Reset() => this.iterator.Reset();

        public readonly void Dispose()
        { }
    }
}
