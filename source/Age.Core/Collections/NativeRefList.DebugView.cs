using System.Diagnostics;

namespace Age.Core.Collections;

public ref partial struct NativeRefList<T> where T : unmanaged
{
    internal ref struct DebugView(NativeRefList<T> source)
    {
        private NativeRefList<T> source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [.. this.source];
    }
}
