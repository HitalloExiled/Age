using System.Diagnostics;

namespace Age.Core.Collections;

public ref partial struct NativeStringRefArray
{
    internal ref struct DebugView(NativeStringRefArray source)
    {
        private NativeStringRefArray source = source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public readonly string[] Elements => this.source.ToArray();
    }
}
