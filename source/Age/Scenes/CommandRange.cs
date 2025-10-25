namespace Age.Scenes;

public readonly partial record struct CommandRange
{
    public readonly ShortRange Pre  { get; init; }
        public readonly ShortRange Post { get; init; }

        public readonly Range FullRange => new(this.Pre.Start, this.Post.End);
        public readonly int   Length    => this.Post.End - this.Pre.Start;

        public CommandRange(int index)
        {
            this.Pre  = new(index);
            this.Post = new(index);
        }

        public CommandRange(int preStart, int preEnd)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(preEnd);
        }

        public CommandRange(int preStart, int preEnd, int post)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(post);
        }

        public CommandRange(int preStart, int preEnd, int postStart, int postEnd)
        {
            this.Pre  = new(preStart, preEnd);
            this.Post = new(postStart, postEnd);
        }

        public CommandRange(ShortRange pre, ShortRange post)
        {
            this.Pre  = pre;
            this.Post = post;
        }

        public readonly CommandRange WithClamp(int value) =>
            new(this.Pre, this.Post.WithClamp(value));

        public readonly CommandRange WithOffset(int offset) =>
            new(this.Pre.WithOffset(offset), this.Post.WithOffset(offset));

        public readonly CommandRange WithPreEnd(int index) =>
            new(this.Pre.Start, index, int.Max(index, this.Post.Start), int.Max(index, this.Post.End));

        public readonly CommandRange WithPost(int index) =>
            new(this.Pre, new(index));

        public readonly CommandRange WithPostEnd(int index) =>
            new(this.Pre.Start, int.Min(index, this.Pre.End), int.Min(index, this.Post.Start), index);

        public readonly CommandRange WithPostOffset(int offset) =>
            new(this.Pre, this.Post.WithOffset(offset));

        public readonly CommandRange WithPostResize(int offset) =>
            new(this.Pre, this.Post.WithResize(offset));

        public readonly CommandRange WithPostStart(int index) =>
            new(this.Pre.Start, int.Min(index, this.Pre.End), index, int.Max(index, this.Post.End));

        public override readonly string ToString() =>
            this.Pre == this.Post ? $"{this.Pre}" : $"{this.Pre}..{this.Post}";
}
