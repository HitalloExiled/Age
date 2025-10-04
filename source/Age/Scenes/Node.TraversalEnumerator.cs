using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Age.Scenes;

public abstract partial class Node
{
	public struct TraversalEnumerator : IEnumerator<Node>
    {
        #region 8-bytes
        public event Action<Node>? SubtreeTraversed;
        private readonly Node root;
        private Node current;
        #endregion

        #region 1-byte
        private bool skipToNextSibling;
        #endregion

        public readonly Node Current => this.current;

        readonly object IEnumerator.Current => this.Current;

        public TraversalEnumerator(Node root)
        {
            this.root = root;

            this.Reset();
        }

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        public bool MoveNext()
        {
            if (!this.skipToNextSibling && this.current.FirstChild is Node firstChild)
            {
                this.current = firstChild;

                return true;
            }
            else if (this.current == this.root)
            {
                return false;
            }

            this.skipToNextSibling = false;

            while (true)
            {
                if (this.current.NextSibling is Node nextSibling)
                {
                    this.current = nextSibling;

                    return true;
                }

                this.current = this.current.Parent!;

                if (this.current == this.root)
                {
                    return false;
                }

                SubtreeTraversed?.Invoke(this.current);
            }
        }

        [MemberNotNull(nameof(current))]
        public void Reset()
        {
            this.current           = this.root;
            this.skipToNextSibling = false;
        }

        public void SkipToNextSibling() =>
            this.skipToNextSibling = true;
    }
}
