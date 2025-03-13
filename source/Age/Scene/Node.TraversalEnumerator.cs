using System.Collections;

namespace Age.Scene;

public abstract partial class Node
{
	public struct TraversalEnumerator : IEnumerator<Node>, IEnumerable<Node>
    {
        #region 8-bytes
        private readonly Node root;
        private Node? current;
        #endregion

        #region 1-byte
        private bool  skipToNextSibling;
        #endregion

        public TraversalEnumerator(Node root)
        {
            this.root = root;

            this.Reset();
        }

        public readonly Node Current => this.current!;

        readonly object IEnumerator.Current => this.Current;

        readonly IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        public readonly void Dispose()
        { }

        public readonly IEnumerator<Node> GetEnumerator() => this;

        public bool MoveNext()
        {
            if (!this.skipToNextSibling && this.current!.FirstChild != null)
            {
                this.current = this.current.FirstChild;

                return true;
            }

            this.skipToNextSibling = false;

            do
            {
                if (this.current!.NextSibling != null)
                {
                    this.current = this.current.NextSibling;

                    return true;
                }

                this.current = this.current == this.root ? null : this.current.Parent;
            }
            while (this.current != null);

            return false;
        }

        public void Reset()
        {
            this.current           = this.root;
            this.skipToNextSibling = false;
        }

        public void SkipToNextSibling() =>
            this.skipToNextSibling = true;
    }
}
