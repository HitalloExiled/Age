namespace Age.Core.Collections;

internal partial struct UnsafeHashCollection
{
    internal enum EntryState
    {
        None = 0,
        Free = 1,
        Used = 2,
    }
}
