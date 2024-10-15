using System.Collections;

namespace Age.Elements;

internal partial class Layer
{
    public struct Enumerator(Layer node) : IEnumerator<Layer>
    {
        private Layer? current;
        private bool  first = true;

        public readonly Layer Current => this.current ?? throw new InvalidOperationException();
        public readonly Layer Node    => node;

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose()
        { }

        public bool MoveNext()
        {
            if (this.first)
            {
                this.current = node.FirstChild;
                this.first   = false;
            }
            else
            {
                this.current = this.current?.NextSibling;
            }

            return this.current != null;
        }

        public void Reset()
        {
            this.first   = true;
            this.current = null;
        }
    }
}
