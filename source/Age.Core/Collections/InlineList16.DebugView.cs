using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct InlineList16<T>
{
    public sealed class DebugView(InlineList16<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Elements => [..source.AsSpan()];
    }
}
