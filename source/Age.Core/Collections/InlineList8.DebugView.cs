using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct InlineList8<T>
{
    public sealed class DebugView(InlineList8<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Elements => [..source.AsSpan()];
    }
}
