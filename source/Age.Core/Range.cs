using System.Numerics;

namespace Age.Core;

public readonly record struct Range<T> : IEquatable<Range<T>> where T : INumber<T>
{
    public readonly T Start { get; }
    public readonly T End   { get; }

    public readonly T Length => this.End - this.Start;

    public Range(T index)
    {
        this.Start = index;
        this.End   = index;
    }

    public Range(T start, T end)
    {
        this.Start = start;
        this.End   = end;
    }

    public readonly Range<T> WithStart(T start) =>
        new(start, this.End);

    public readonly Range<T> WithEnd(T end) =>
        new(this.Start, end);

    public readonly bool Contains(Range<T> range) =>
        this.Start <= range.Start && this.End >= range.End;

    public static implicit operator Range(Range<T> range) =>
        new(int.CreateChecked(range.Start), int.CreateChecked(range.End));

    public override readonly string ToString() =>
        $"{this.Start}..{this.End}";
}
