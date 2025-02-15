namespace Age.Elements.Layouts;

public record struct TextSelection(uint Start, uint End)
{
    public uint Start = Start;
    public uint End   = End;

    public readonly bool Inverted => this.Start > this.End;
    public readonly int  Length   => (int)this.End - (int)this.Start;

    public readonly TextSelection Ordered() => this.Inverted ? new(this.End, this.Start) : this;
    public readonly TextSelection WithStart(uint start) => new(start, this.End);
    public readonly TextSelection WithEnd(uint end) => new(this.Start, end);

    public override readonly string ToString() =>
        $"[{this.Start}..{this.End}]";
}
