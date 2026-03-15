using System.Diagnostics;

namespace Age.Core.Collections;

public ref partial struct NativeRefArray<T> where T : unmanaged
{
    internal ref struct DebugView(NativeRefArray<T> source)
    {
        private NativeRefArray<T> source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [.. this.source];
    }
}
