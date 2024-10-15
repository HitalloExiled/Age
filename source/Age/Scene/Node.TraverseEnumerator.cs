using System.Collections;

namespace Age.Scene;

public abstract partial class Node
{
	public struct TraverseEnumerator(Node root) : IEnumerator<Node>, IEnumerable<Node>
    {
        private readonly Node root = root;

        private Node? current;
        private bool  first = true;
        private bool  skipToNextSibling;

        internal readonly Node? UnsafeCurrent => this.current;

        public readonly Node Current => this.current ?? throw new NullReferenceException();

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        public bool MoveNext()
        {
            if (this.first)
            {
                this.first   = false;
                this.current = this.root.FirstChild;
            }
            else if (this.current!.FirstChild != null && !this.skipToNextSibling)
            {
                this.current = this.current.FirstChild;
            }
            else
            {
                this.skipToNextSibling = false;

                while (this.current != null)
                {
                    if (this.current.NextSibling != null)
                    {
                        this.current = this.current.NextSibling;

                        break;
                    }

                    this.current = this.current == this.root ? null : this.current.Parent;
                }
            }

            return this.current != null;
        }

        public void SkipToNextSibling() =>
            this.skipToNextSibling = true;

        public void Reset()
        {
            this.skipToNextSibling = false;
            this.first             = true;
            this.current           = null;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
