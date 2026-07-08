using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

internal unsafe partial struct UnsafeHashCollection
{
    internal struct Enumerator(UnsafeHashCollection* collection)
    {
        private int index = -1;

        public Entry*                Current    = null;
        public UnsafeHashCollection* Collection = collection;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            while (++this.index < this.Collection->UsedCount)
            {
                var entry = GetEntry(this.Collection, this.index);

                if (entry->State == EntryState.Used)
                {
                    this.Current = entry;
                    return true;
                }
            }

            this.Current = null;
            return false;
        }

        public void Reset() => this.index = -1;
    }
}
