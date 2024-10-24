using System.Collections;

namespace Age.Scene;

public abstract partial class Node
{
	public struct TraverseEnumerator(Node root) : IEnumerator<Node>, IEnumerable<Node>
    {
        private readonly Node root = root;
        private bool  first = true;
        private bool  skipToNextSibling;

        internal Node? CurrentNode { get; private set; }

        public readonly Node Current => this.CurrentNode ?? throw new NullReferenceException();

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
                this.CurrentNode = this.root.FirstChild;
            }
            else if (this.CurrentNode!.FirstChild != null && !this.skipToNextSibling)
            {
                this.CurrentNode = this.CurrentNode.FirstChild;
            }
            else
            {
                this.skipToNextSibling = false;

                while (this.CurrentNode != null)
                {
                    if (this.CurrentNode.NextSibling != null)
                    {
                        this.CurrentNode = this.CurrentNode.NextSibling;

                        break;
                    }

                    this.CurrentNode = this.CurrentNode == this.root ? null : this.CurrentNode.Parent;
                }
            }

            return this.CurrentNode != null;
        }

        public void Reset()
        {
            this.skipToNextSibling = false;
            this.first             = true;
            this.CurrentNode           = null;
        }

        public void SkipToNextSibling() =>
            this.skipToNextSibling = true;
    }
}
