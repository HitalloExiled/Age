namespace Age.Core.Extensions;

public static partial class Extension
{
    public static bool EqualsTo(this Range? range, Range? other) =>
        range.HasValue == other.HasValue && (!range.HasValue || range.Value.Equals(other!.Value));

    public static Range WithStart(this Range range, int start) =>
        new(start, range.End);

    public static Range WithStart(this Range range, Index start) =>
        new(start, range.End);

    public static Range WithEnd(this Range range, int end) =>
        new(range.Start, end);

    public static Range WithEnd(this Range range, Index end) =>
        new(range.Start, end);

    public static Range Ordered(this Range range) =>
        range.Start.Value < range.End.Value
        ? range
        : new(range.End, range.Start);
}
