using System.Collections;

namespace Age.Scene;

public abstract partial class Node
{
	public struct TraverseEnumerator(Node root) : IEnumerator<Node>, IEnumerable<Node>
    {
        #region 8-bytes
        private readonly Node root = root;
        private Node? current;
        #endregion

        #region 1-byte
        private bool  first = true;
        private bool  skipToNextSibling;
        #endregion

        public readonly Node Current => this.current!;

        readonly object IEnumerator.Current => this.Current;

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

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

        public void Reset()
        {
            this.skipToNextSibling = false;
            this.first             = true;
            this.current           = null;
        }

        public void SkipToNextSibling() =>
            this.skipToNextSibling = true;
    }
}
