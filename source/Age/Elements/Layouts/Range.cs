namespace Age.Elements.Layouts;

public record struct TextSelection(uint Start, uint End)
{
    public uint Start = Start;
    public uint End   = End;

    public readonly int Offset => (int)this.End - (int)this.Start;

    public readonly TextSelection Ordered() => this.Start < this.End
        ? this
        : new(this.End, this.Start);

    public readonly TextSelection WithStart(uint start) => new(start, this.End);
    public readonly TextSelection WithEnd(uint end) => new(this.Start, end);
}

