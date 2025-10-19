namespace Age.Scenes;

public readonly partial record struct CommandRange
{
    public readonly record struct Variant
    {
        public readonly ShortRange Pre  { get; init; }
        public readonly ShortRange Post { get; init; }

        public readonly Range FullRange => new(this.Pre.Start, this.Post.End);
        public readonly int   Length    => this.Post.End - this.Pre.Start;

        public Variant(int index)
        {
            this.Pre  = new(index);
            this.Post = new(index);
        }

        public Variant(int preStart, int preEnd)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(preEnd);
        }

        public Variant(int preStart, int preEnd, int post)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(post);
        }

        public Variant(int preStart, int preEnd, int postStart, int postEnd)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(postStart, postEnd);
        }

        public Variant(ShortRange pre, ShortRange post)
        {
            this.Pre  = pre;
            this.Post = post;
        }

        public readonly Variant WithClamp(int value) =>
            new(this.Pre, this.Post.WithClamp(value));

        public readonly Variant WithOffset(int offset) =>
            new(this.Pre.WithOffset(offset), this.Post.WithOffset(offset));

        public readonly Variant WithPreEnd(int index) =>
            new(this.Pre.Start, index, int.Max(index, this.Post.Start), int.Max(index, this.Post.End));

        public readonly Variant WithPost(int index) =>
            new(this.Pre, new(index));

        public readonly Variant WithPostEnd(int index) =>
            new(this.Pre.Start, int.Min(index, this.Pre.End), int.Min(index, this.Post.Start), index);

        public readonly Variant WithPostOffset(int offset) =>
            new(this.Pre, this.Post.WithOffset(offset));

        public readonly Variant WithPostResize(int offset) =>
            new(this.Pre, this.Post.WithResize(offset));

        public readonly Variant WithPostStart(int index) =>
            new(this.Pre.Start, int.Min(index, this.Pre.End), index, int.Max(index, this.Post.End));

        public override readonly string ToString() =>
            this.Pre == this.Post ? $"{this.Pre}" : $"{this.Pre}..{this.Post}";
    }
}
