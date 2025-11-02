using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Age.Scenes;

public abstract partial class Node
{
    internal struct CompositeTraversalEnumerator : IEnumerator<Node>, IEnumerable<Node>
    {
        #region 8-bytes
        public event Action<Node>? SubtreeTraversed;

        private readonly Node root;

        private Node current;
        #endregion

        #region 1-byte
        private bool skipToNextSibling;
        #endregion

        public CompositeTraversalEnumerator(Node root)
        {
            this.root = root;

            this.Reset();
        }

        public readonly Node Current => this.current!;

        readonly object IEnumerator.Current => this.Current;

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        readonly IEnumerator<Node> IEnumerable<Node>.GetEnumerator() =>
            this.GetEnumerator();

        public readonly void Dispose()
        { }

        public readonly CompositeTraversalEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if (this.current.IsCompositeLeaf && this.current != this.root)
            {
                this.SubtreeTraversed?.Invoke(this.current);
            }

            if (!this.skipToNextSibling && (this.current.ShadowRoot ?? this.current.FirstChild) is Node firstChild)
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
                if ((this.current.NextSibling ?? this.current.ShadowHost?.FirstChild) is Node nextSibling)
                {
                    this.current = nextSibling;

                    return true;
                }

                var parent = this.current.CompositeParent!;

                if (parent == this.root)
                {
                    return false;
                }

                this.current = parent;

                this.SubtreeTraversed?.Invoke(parent);
            }
        }

        [MemberNotNull(nameof(current))]
        public void Reset()
        {
            this.skipToNextSibling = false;
            this.current           = this.root;
        }

        public void SkipToNextSibling() =>
            this.skipToNextSibling = true;
    }
}
