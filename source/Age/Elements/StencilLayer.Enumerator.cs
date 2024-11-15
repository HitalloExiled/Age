using System.Collections;

namespace Age.Elements;

internal sealed partial class StencilLayer
{
    public struct Enumerator(StencilLayer node) : IEnumerator<StencilLayer>
    {
        private StencilLayer? current;
        private bool  first = true;

        public readonly StencilLayer Current => this.current!;
        public readonly StencilLayer Node    => node;

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
