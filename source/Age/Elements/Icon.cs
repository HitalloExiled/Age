using Age.Elements.Layouts;
using Age.Numerics;

namespace Age.Elements;

public class Icon : Styleable
{
    public override string NodeName => nameof(Icon);

    internal override IconLayout Layout { get; }

    public string? IconName
    {
        get => this.Layout.IconName;
        set => this.Layout.IconName = value;
    }

    public Icon(string? iconName = null, ushort? size = null, Color? color = null)
    {
        this.Layout = new(this);

        this.Style = new()
        {
            Color    = color,
            FontSize = size,
        };

        this.IconName = iconName;
    }

    protected sealed override void OnIndexed()
    {
        base.OnIndexed();
        this.Layout.HandleTargetIndexed();
    }
}
