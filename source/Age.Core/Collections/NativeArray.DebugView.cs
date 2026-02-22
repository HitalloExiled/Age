using System.Diagnostics;

namespace Age.Core.Collections;

public partial class NativeArray<T> where T : unmanaged
{
    internal sealed class DebugView(NativeArray<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Elements => [..source];
    }
}
