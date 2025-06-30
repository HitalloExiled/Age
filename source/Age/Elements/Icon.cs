using Age.Elements.Layouts;

namespace Age.Elements;

public class Icon : Layoutable
{
    public override string NodeName => nameof(Icon);

    internal override IconLayout Layout { get; }

    public string? IconName
    {
        get => this.Layout.IconName;
        set => this.Layout.IconName = value;
    }

    public Icon() => this.Layout = new(this);
}
