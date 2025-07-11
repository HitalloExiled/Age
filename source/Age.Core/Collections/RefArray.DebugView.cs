using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe ref partial struct RefArray<T> where T : unmanaged
{
    internal ref struct DebugView(RefArray<T> source)
    {
        private RefArray<T> source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [.. this.source];
    }
}
