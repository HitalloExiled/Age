using System.Diagnostics;

namespace Age.Core.Interop;

public unsafe partial class NativeArray<T> where T : unmanaged
{
    internal class DebugView(NativeArray<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Elements => [..source];
    }
}
