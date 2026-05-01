using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe partial struct NativeDictionary<K, V> where K : unmanaged, IEquatable<K>
where V : unmanaged
{
    public readonly partial struct KeyCollection
    {
        internal struct DebugView(KeyCollection source)
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public readonly K[] Elements => source.ToArray();
        }
    }
}
