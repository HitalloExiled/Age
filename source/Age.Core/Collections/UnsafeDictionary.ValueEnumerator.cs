using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeDictionary
{
    public struct ValueEnumerator<V>(UnsafeDictionary* dictionary)
    where V : unmanaged, allows ref struct
    {
        private UnsafeHashCollection.Enumerator iterator = new(&dictionary->collection);
        private readonly int valueOffset = dictionary->valueOffset;

        public readonly V Current
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
