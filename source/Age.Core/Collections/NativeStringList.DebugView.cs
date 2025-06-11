using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe partial class NativeStringList
{
    internal readonly ref struct DebugView(NativeStringList source)
    {
        private readonly NativeStringList source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly string[] Elements => [.. this.source];
    }
}
