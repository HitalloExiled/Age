namespace Age.Core.Collections;

internal partial struct UnsafeHashCollection
{
    public unsafe struct Entry
    {
        public const int ALIGNMENT = 8;

        public Entry*     Next;
        public int        Hash;
        public EntryState State;
    }
}
