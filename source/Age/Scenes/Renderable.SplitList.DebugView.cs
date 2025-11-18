using System.Diagnostics;

namespace Age.Scenes;

public abstract partial class Renderable<T>
{
    internal partial struct SplitList
    {
        private ref struct DebugView(SplitList source)
        {
            private SplitList source = source;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public readonly T[] Elements => [.. this.source.AsSpan()];
        }
    }
}
