using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeDictionary
{
    public struct KeyEnumerator<K>(UnsafeDictionary* dictionary)
    where K : unmanaged, IEquatable<K>
    {
        private UnsafeHashCollection.Enumerator iterator = new(&dictionary->collection);

        private readonly int keyOffset = dictionary->collection.KeyOffset;

        public readonly K Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(this.iterator.Current != null);

                return *(K*)((byte*)this.iterator.Current + this.keyOffset);
            }
        }

        public bool MoveNext() => this.iterator.MoveNext();

        public void Reset() => this.iterator.Reset();

        public readonly void Dispose()
        { }
    }
}
