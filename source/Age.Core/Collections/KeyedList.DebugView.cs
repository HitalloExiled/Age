using System.Diagnostics;

namespace Age.Core.Collections;

public partial class KeyedList<TKey, TValue> where TKey : unmanaged, Enum
where TValue : notnull
{
    internal sealed class DebugView(KeyedList<TKey, TValue> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Elements => [..source];
    }
}
