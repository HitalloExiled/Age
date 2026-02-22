namespace Age.Commands;

public sealed partial record TextCommand : RectCommand
{
    public int Index
    {
        get => (int)this.UserData;
        set => this.UserData = value;
    }

    public int           Line      { get; set; }
    public SurrogateKind Surrogate { get; set; }

    public override void Reset()
    {
        base.Reset();

        this.Surrogate = default;
        this.Index     = default;
        this.Line      = default;
    }
}
