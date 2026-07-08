using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe partial struct NativeStack<T> where T : unmanaged
{
    internal struct DebugView(NativeStack<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[]? Elements => source.IsCreated ? source.ToArray() : null;
    }
}
