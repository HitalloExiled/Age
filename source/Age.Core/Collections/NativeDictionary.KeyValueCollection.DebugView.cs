using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe partial struct NativeDictionary<K, V> where K : unmanaged, IEquatable<K>
where V : unmanaged
{
    public readonly partial struct KeyValueCollection
    {
        internal struct DebugView(KeyValueCollection source)
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public readonly KeyValuePair<K, V>[] Elements => source.ToArray();
        }
    }
}
