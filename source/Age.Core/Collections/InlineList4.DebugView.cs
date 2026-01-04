using System.Diagnostics;

namespace Age.Core.Collections;

public partial struct InlineList4<T>
{
    public sealed class DebugView(InlineList4<T> source)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Elements => [..source.AsSpan()];
    }
}
