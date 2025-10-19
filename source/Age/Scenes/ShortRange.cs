using System.Diagnostics;

namespace Age.Scenes;

public readonly record struct ShortRange
{
    private readonly ushort start;
    private readonly ushort end;

    public readonly int Start => this.start;
    public readonly int End   => this.end;

    public readonly int Length => this.end - this.start;

    public ShortRange(int index)
    {
        this.start = (ushort)index;
        this.end   = (ushort)index;
    }

    public ShortRange(int start, int end)
    {
        Debug.Assert(start <= end);

        this.start = (ushort)start;
        this.end   = (ushort)end;
    }

    public static ShortRange CreateWithLength(int start, int length) =>
        new(start, start + length);

    public readonly bool Contains(ShortRange range) =>
        this.start <= range.start && this.end >= range.end;

    public readonly ShortRange WithClamp(int value) =>
        new(int.Min(this.start, value), int.Min(this.end, value));

    public readonly ShortRange WithEnd(int end) =>
        new(int.Min(this.start, end), end);

    public ShortRange WithEndOffset(int offset) =>
        new(this.start, this.end + offset);

    public readonly ShortRange WithLength(int length) =>
        new(this.start, this.start + length);

    public readonly ShortRange WithResize(int length) =>
        new(this.start, this.end + length);

    public readonly ShortRange WithOffset(int offset) =>
        new(this.start + offset, this.end + offset);

    public readonly ShortRange WithStart(int start) =>
        new(start, this.end);

    public override readonly string ToString() =>
        this.start == this.end ? $"{this.start}" : $"[{this.start}..{this.end}]";

    public static implicit operator Range(ShortRange range) =>
        new(range.start, range.end);
}
