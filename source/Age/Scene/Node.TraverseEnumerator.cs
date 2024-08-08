using System.Collections;

namespace Age.Scene;

public abstract partial class Node
{
	public struct TraverseEnumerator(Node root) : IEnumerator<Node>, IEnumerable<Node>
    {
        private readonly Node root  = root;

        private bool  first = true;
        private Node? current;

        public readonly Node Current => this.current ?? throw new InvalidOperationException();

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        public bool MoveNext()
        {
            if (this.first)
            {
                this.current = this.root.FirstChild;
                this.first   = false;
            }
            else
            {
                if (this.current!.FirstChild != null)
                {
                    this.current = this.current.FirstChild;
                }
                else if (this.current.NextSibling != null)
                {
                    this.current = this.current.NextSibling;
                }
                else if (this.current.Parent != null)
                {
                    do
                    {
                        if (this.current.Parent?.NextSibling != null)
                        {
                            this.current = this.current.Parent.NextSibling;

                            break;
                        }
                        else
                        {
                            this.current = this.current.Parent;
                        }
                    }
                    while (this.current != null);
                }
                else
                {
                    this.current = null;
                }
            }

            return this.current != null;
        }

        public void Reset()
        {
            this.first   = true;
            this.current = null;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
