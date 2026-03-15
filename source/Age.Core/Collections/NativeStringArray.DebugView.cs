using System.ComponentModel;
using System.Diagnostics;

namespace Age.Core.Collections;

public partial class NativeStringArray
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly ref struct DebugView(NativeStringArray source)
    {
        private readonly NativeStringArray source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly string[] Elements => this.source.ToArray();
    }
}
