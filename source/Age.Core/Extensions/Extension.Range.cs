namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(Range? range)
    {
        public bool EqualsTo(Range? other) =>
        range.HasValue == other.HasValue && (!range.HasValue || range.Value.Equals(other!.Value));
    }

    extension(Range range)
    {
        public Range WithStart(int start) =>
        new(start, range.End);

        public Range WithStart(Index start) =>
            new(start, range.End);

        public Range WithEnd(int end) =>
            new(range.Start, end);

        public Range WithEnd(Index end) =>
            new(range.Start, end);

        public Range Ordered() =>
            range.Start.Value < range.End.Value
                ? range
                : new(range.End, range.Start);
    }
}
