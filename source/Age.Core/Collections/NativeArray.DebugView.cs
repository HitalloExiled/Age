using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct NativeArray<T> where T : unmanaged
{
    internal struct DebugView(NativeArray<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => source.ToArray();
    }
}
