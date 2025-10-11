namespace Age.Scenes;

public readonly partial record struct CommandRange
{
    public readonly record struct Variant
    {
        public readonly ShortRange Pre  { get; init; }
        public readonly ShortRange Post { get; init; }

        public readonly int Length => this.Post.End - this.Pre.Start;

        public Variant(ushort index)
        {
            this.Pre  = new(index);
            this.Post = new(index);
        }

        public Variant(ushort preStart, ushort preEnd)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(preEnd);
        }

        public Variant(ushort preStart, ushort preEnd, ushort post)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(post);
        }

        public Variant(ushort preStart, ushort preEnd, ushort postStart, ushort postEnd)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(postStart, postEnd);
        }

        public Variant(ShortRange pre, ShortRange post)
        {
            this.Pre  = pre;
            this.Post = post;
        }

        public readonly Variant WithClamp(ushort value) =>
            new(this.Pre, this.Post.WithClamp(value));

        public readonly Variant WithOffset(short offset) =>
            new(this.Pre.WithOffset(offset), this.Post.WithOffset(offset));

        public readonly Variant WithPreEnd(ushort index) =>
            new(this.Pre.Start, index, ushort.Max(index, this.Post.Start), ushort.Max(index, this.Post.End));

        public readonly Variant WithPost(ushort index) =>
            new(this.Pre, new(index));

        public readonly Variant WithPostEnd(ushort index) =>
            new(this.Pre.Start, ushort.Min(index, this.Pre.End), ushort.Min(index, this.Post.Start), index);

        public readonly Variant WithPostOffset(short offset) =>
            new(this.Pre, this.Post.WithOffset(offset));

        public readonly Variant WithPostResize(short offset) =>
            new(this.Pre, this.Post.WithResize(offset));

        public readonly Variant WithPostStart(ushort index) =>
            new(this.Pre.Start, ushort.Min(index, this.Pre.End), index, ushort.Max(index, this.Post.End));

        public override readonly string ToString() =>
            this.Pre == this.Post ? $"{this.Pre}" : $"{this.Pre}..{this.Post}";

        public static implicit operator Range(Variant shortRange) => new(shortRange.Pre.Start, shortRange.Post.End);
    }
}
