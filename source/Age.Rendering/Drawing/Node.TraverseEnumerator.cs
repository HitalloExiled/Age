using System.Collections;

namespace Age.Rendering.Drawing;

public abstract partial class Node
{
	public struct TraverseEnumerator(Node root, bool topDown = false) : IEnumerator<Node>, IEnumerable<Node>
    {
        private readonly Node root    = root;
        private readonly bool topDown = topDown;

        private int   state;
        private Node? current;

        public readonly Node Current => this.current ?? throw new InvalidOperationException();

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        public bool MoveNext()
        {
            switch (this.state)
            {
                case 0:
                    if (this.topDown)
                    {
                        this.current = this.root;
                        this.state   = 2;
                    }
                    else
                    {
                        var node = this.root;

                        if (node.FirstChild != null)
                        {
                            do
                            {
                                node = node.FirstChild;
                            }
                            while (node.FirstChild != null);
                        }

                        this.current = node;
                        this.state   = 1;
                    }


                    break;
                case 1:
                    if (this.topDown)
                    {
                        if (this.current!.NextSibling != null)
                        {
                            this.current = this.current.NextSibling;
                        }
                        else
                        {
                            var next = this.current;

                            while (next != null)
                            {
                                if (next.Parent?.NextSibling != null)
                                {
                                    next = next.Parent.NextSibling;

                                    break;
                                }
                                else
                                {
                                    next = next.Parent;
                                }
                            }

                            this.current = next;
                            this.state   = 2;
                        }
                    }
                    else
                    {
                        if (this.current!.NextSibling != null)
                        {
                            var node = this.current.NextSibling;

                            if (node.FirstChild != null)
                            {
                                do
                                {
                                    node = node.FirstChild;
                                }
                                while (node.FirstChild != null);
                            }

                            this.current = node;
                        }
                        else
                        {
                            goto case 2;
                        }
                    }

                    break;
                case 2:
                    if (this.topDown)
                    {
                        if (this.current!.FirstChild != null)
                        {
                            this.current = this.current!.FirstChild;
                        }
                        else
                        {
                            this.state = 1;
                            goto case 1;
                        }
                    }
                    else
                    {
                        this.current = this.current!.Parent;
                        this.state = 1;
                    }

                    break;
            }

            return this.current != null;
        }

        public void Reset()
        {
            this.state   = 0;
            this.current = null;
        }

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
