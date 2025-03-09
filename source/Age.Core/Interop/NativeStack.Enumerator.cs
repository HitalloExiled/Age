using System.Collections;

namespace Age.Core.Interop;

public unsafe partial class NativeStack<T> where T : unmanaged
{
    public struct Enumerator(NativeStack<T> stack) : IEnumerator<T>
    {
        private readonly NativeStack<T> stack = stack;
        private int index = stack.Count;

        public T Current => this.stack.buffer[this.index];

        object IEnumerator.Current => this.Current;

        public readonly void Dispose() { }
        public bool MoveNext() => --this.index > -1;
        public void Reset() => this.index = this.stack.Count;
    }
}
