using System.Diagnostics;

namespace Age.Core.Interop;

public unsafe partial class NativeList<T> where T : unmanaged
{
    internal class DebugView(NativeList<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Elements => [..source];
    }
}
