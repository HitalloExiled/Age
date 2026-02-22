using System.Diagnostics;

namespace Age.Core.Collections;

public ref partial struct RefList<T> where T : unmanaged
{
    internal ref struct DebugView(RefList<T> source)
    {
        private RefList<T> source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [.. this.source];
    }
}
