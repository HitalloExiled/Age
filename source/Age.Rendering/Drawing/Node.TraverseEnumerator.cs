using System.Collections;
using System.Runtime.CompilerServices;

namespace Age.Rendering.Drawing;

public abstract partial class Node
{
	public struct TraverseEnumerator(Node root, bool topDown = false) : IEnumerator<Node>, IEnumerable<Node>
    {
        private readonly Node root    = root;
        private readonly bool topDown = topDown;

        private bool  first = true;
        private Node? current;

        public readonly Node Current => this.current ?? throw new InvalidOperationException();

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Node GetDeepest(Node node)
        {
            while (node.FirstChild != null)
            {
                node = node.FirstChild;
            }

            return node;
        }

        public bool MoveNext()
        {
            if (this.first)
            {
                this.current = this.topDown ? this.root.FirstChild : GetDeepest(this.root);
                this.first   = false;
            }
            else if (this.topDown)
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
            else
            {
                this.current = this.current!.NextSibling != null
                    ? GetDeepest(this.current.NextSibling)
                    : this.current!.Parent != this.root
                        ? this.current.Parent
                        : null;
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
