using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct NativeDictionary<K, V> where K : unmanaged, IEquatable<K>
where V : unmanaged
{
    internal struct DebugView(NativeDictionary<K, V> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly KeyValuePair<K, V>[]? Elements => !source.IsCreated ? null : source.Entries.ToArray();
    }
}
