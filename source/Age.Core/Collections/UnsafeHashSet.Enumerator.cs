using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeHashSet
{
    public struct Enumerator<T> where T : unmanaged
    {
        private UnsafeHashCollection.Enumerator iterator;
        private readonly int                     keyOffset;

        public Enumerator(UnsafeHashSet* set)
        {
            this.keyOffset = set->collection.KeyOffset;
            this.iterator  = new(&set->collection);
        }

        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => *(T*)((byte*)this.iterator.Current + this.keyOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => this.iterator.MoveNext();

        public void Reset() => this.iterator.Reset();
    }
}
