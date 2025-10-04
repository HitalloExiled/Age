namespace Age.Scenes;

public readonly partial record struct CommandRange
{
    public readonly record struct Segment
    {
        public readonly ushort Start  { get; }
        public readonly ushort End    { get; }
        public readonly ushort Extend { get; }

        public readonly int Length         => this.End - this.Start;
        public readonly int ExtendedLength => this.Extend - this.Start;

        public Range Range         => new(this.Start, this.End);
        public Range ExtendedRange => new(this.Start, this.Extend);

        public Segment(ushort index)
        {
            this.Start  = index;
            this.End    = index;
            this.Extend = index;
        }

        public Segment(ushort start, ushort end)
        {
            this.Start  = start;
            this.End    = end;
            this.Extend = end;
        }

        public Segment(ushort start, ushort end, ushort extend)
        {
            this.Start  = start;
            this.End    = end;
            this.Extend = extend;
        }

        public readonly Segment WithEnd(ushort end) =>
            new(this.Start, end, ushort.Max(end, this.Extend));

        public readonly Segment WithExtend(ushort extend) =>
            new(ushort.Min(this.Start, extend), ushort.Min(this.End, extend), extend);

        public readonly Segment WithExtendOffset(short offset) =>
            new(this.Start, this.End, (ushort)(this.Extend + offset));

        public readonly Segment WithOffset(short offset) =>
            new((ushort)(this.Start + offset), (ushort)(this.End + offset), (ushort)(this.Extend + offset));

        public readonly bool Contains(Segment range) =>
            this.Start <= range.Start && this.Extend >= range.Extend;

        public override readonly string ToString() =>
            this.Start == this.Extend
                ? $"[{this.Start}]"
                : this.Start == this.End
                    ? $"[{this.Start}]..{this.Extend}"
                    : this.End == this.Extend
                        ? $"[{this.Start}..{this.Extend}]"
                        : $"[{this.Start}..{this.End}]..{this.Extend}";
    }
}
