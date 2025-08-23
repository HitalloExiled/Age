using Age.Core.Extensions;

namespace Age.Elements;

internal partial record struct ComposedPath
{
    internal ref struct Enumerator
    {
        private readonly ReadOnlySpan<Element> leftToAncestor;
        private readonly ReadOnlySpan<Element> rightToAncestor;

        private int  index = -1;
        private bool left  = true;

        public Enumerator(ReadOnlySpan<Element> leftToAncestor, ReadOnlySpan<Element> rightToAncestor)
        {
            this.leftToAncestor = leftToAncestor;
            this.rightToAncestor = rightToAncestor;
        }

        public readonly Element Current => this.left ? this.leftToAncestor[this.index] : this.rightToAncestor[this.index];

        public bool MoveNext()
        {
            if (this.left)
            {
                if (++this.index < this.leftToAncestor.Length)
                {
                    return true;
                }

                this.left  = false;
                this.index = this.rightToAncestor.Length;
            }

            return --this.index > -1;
        }
    }
}
