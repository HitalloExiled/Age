namespace Age.Elements;

internal partial record struct ComposedPath
{
    internal ref struct Enumerator
    {
        private readonly ReadOnlySpan<Layoutable> leftToAncestor;
        private readonly ReadOnlySpan<Layoutable> rightToAncestor;

        private int  index = -1;
        private bool left  = true;

        public Enumerator(ReadOnlySpan<Layoutable> leftToAncestor, ReadOnlySpan<Layoutable> rightToAncestor)
        {
            this.leftToAncestor  = leftToAncestor;
            this.rightToAncestor = rightToAncestor;
        }

        public readonly Layoutable Current => this.left ? this.leftToAncestor[this.index] : this.rightToAncestor[this.index];

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
