using System.Diagnostics;

namespace Age.Core;

public partial record struct NativeBuffer<T> where T : unmanaged
{
    public struct DebugView(NativeBuffer<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly T[] Elements => [..source.AsSpan()];
    }
}
