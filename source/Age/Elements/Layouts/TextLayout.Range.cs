namespace Age.Elements.Layouts;

internal partial class TextLayout
{
    public record struct Range(uint Start, uint End)
    {
        public uint Start = Start;
        public uint End   = End;

        public readonly Range Ordered() => this.Start < this.End
            ? this
            : new(this.End, this.Start);
        public readonly Range WithStart(uint start) => new(start, this.End);
        public readonly Range WithEnd(uint end) => new(this.Start, end);
    }
}
