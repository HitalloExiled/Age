namespace Age.Commands;

public sealed record TextCommand : RectCommand
{
    public int Index { get; set; }
    public int Line  { get; set; }

    public override void Reset()
    {
        base.Reset();

        this.Index = default;
        this.Line  = default;
    }
}
