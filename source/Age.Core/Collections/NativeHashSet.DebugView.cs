using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct NativeHashSet<T>
{
    internal struct DebugView(NativeHashSet<T> set)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Items => set.ToNativeArray().ToArray();
    }
}
