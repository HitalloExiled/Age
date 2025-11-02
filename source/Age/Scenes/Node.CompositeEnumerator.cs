using System.Collections;

namespace Age.Scenes;

public abstract partial class Node
{
    internal struct CompositeEnumerator(Node node) : IEnumerator<Node>
    {
        private Node? current;
        private int   state;

        public readonly Node Current => this.current!;
        public readonly Node Node    => node;

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose()
        { }

        public bool MoveNext()
        {
            switch (this.state)
            {
                case 0:
                    if (node.ShadowRoot is Node shadowRoot)
                    {
                        this.current = shadowRoot;
                        this.state   = 1;
                    }
                    else
                    {
                        this.current = node.FirstChild;
                        this.state   = 2;
                    }

                    break;
                case 1:
                    this.current = this.current!.ShadowHost!.FirstChild;
                    this.state   = 2;

                    break;
                case 2:
                    this.current = this.current?.NextSibling;
                    break;

            }

            return this.current != null;
        }

        public void Reset()
        {
            this.state   = default;
            this.current = null;
        }
    }
}
