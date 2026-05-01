using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe partial struct NativeDictionary<K, V> where K : unmanaged, IEquatable<K>
where V : unmanaged
{
    public readonly partial struct ValueCollection
    {
        internal struct DebugView(ValueCollection source)
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public readonly V[] Elements => source.ToArray();
        }
    }
}
