using System.Diagnostics;

namespace Age.Scenes;

public readonly record struct ShortRange
{
    public readonly ushort Start { get; }
    public readonly ushort End   { get; }

    public readonly int Length => this.End - this.Start;

    public ShortRange(ushort index)
    {
        this.Start = index;
        this.End   = index;
    }

    public ShortRange(ushort start, ushort end)
    {
        Debug.Assert(start <= end);

        this.Start = start;
        this.End   = end;
    }

    public static ShortRange CreateWithLength(ushort start, ushort length) =>
        new(start, (ushort)(start + length));

    public readonly bool Contains(ShortRange range) =>
        this.Start <= range.Start && this.End >= range.End;

    public readonly ShortRange WithClamp(ushort value) =>
        new(ushort.Min(this.Start, value), ushort.Min(this.End, value));

    public readonly ShortRange WithEnd(ushort end) =>
        new(ushort.Min(this.Start, end), end);

    public ShortRange WithEndOffset(short offset) =>
        new(this.Start, (ushort)(this.End + offset));

    public readonly ShortRange WithLength(ushort length) =>
        new(this.Start, (ushort)(this.Start + length));

    public readonly ShortRange WithResize(short length) =>
        new(this.Start, (ushort)(this.End + length));

    public readonly ShortRange WithOffset(short offset) =>
        new((ushort)(this.Start + offset), (ushort)(this.End + offset));

    public readonly ShortRange WithStart(ushort start) =>
        new(start, this.End);

    public override readonly string ToString() =>
        this.Start == this.End ? $"{this.Start}" : $"[{this.Start}..{this.End}]";

    public static implicit operator Range(ShortRange range) =>
        new(range.Start, range.End);
}
