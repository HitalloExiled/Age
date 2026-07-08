using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct NativeList<T>
{
    internal struct DebugView(NativeList<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[]? Elements => source.IsCreated ? source.ToArray() : null;
    }
}
