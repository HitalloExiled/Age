using System.Diagnostics;

namespace Age.Core.Interop;

public unsafe ref partial struct RefList<T> where T : unmanaged
{
    internal ref struct DebugView(RefList<T> source)
    {
        private RefList<T> source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [.. this.source];
    }
}
