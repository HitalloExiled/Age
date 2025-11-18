using System.Diagnostics;

namespace Age.Core.Collections;

public partial class NativeList<T> where T : unmanaged
{
    internal sealed class DebugView(NativeList<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Elements => [..source];
    }
}
