namespace Age.Scenes;

public readonly record struct SubtreeRange
{
    public readonly ushort Start { get; }
    public readonly ushort End   { get; }

    public readonly int Length => this.End - this.Start;

    public SubtreeRange(ushort index)
    {
        this.Start = index;
        this.End   = index;
    }

    public SubtreeRange(ushort start, ushort end)
    {
        this.Start = start;
        this.End   = end;
    }

    public static SubtreeRange CreateWithLength(ushort start, ushort length) =>
        new(start, (ushort)(start + length));

    public readonly bool Contains(SubtreeRange range) =>
        this.Start <= range.Start && this.End >= range.End;

    public readonly SubtreeRange WithEnd(ushort end) =>
        new(ushort.Min(this.Start, end), end);

    public SubtreeRange WithEndOffset(short offset) =>
        new(this.Start, (ushort)(this.End + offset));

    public readonly SubtreeRange WithLength(ushort length) =>
        new(this.Start, (ushort)(this.Start + length));

    public readonly SubtreeRange WithOffset(short offset) =>
        new((ushort)(this.Start + offset), (ushort)(this.End + offset));

    public readonly SubtreeRange WithStart(ushort start) =>
        new(start, this.End);

    public override readonly string ToString() =>
        $"{this.Start}..{this.End}";

    public static implicit operator Range(SubtreeRange range) =>
        new(range.Start, range.End);
}
