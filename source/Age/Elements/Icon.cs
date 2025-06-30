using Age.Elements.Layouts;
using Age.Scene;

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

    public Icon(string iconName) : this() =>
        this.Layout.IconName = iconName;

    protected override void OnAdopted(Node parent)
    {
        switch (parent)
        {
            case Element parentElement:
                this.Layout.HandleTargetAdopted(parentElement);
                break;

            case ShadowTree shadowTree:
                this.Layout.HandleTargetAdopted(shadowTree.Host);
                break;
        }
    }

    protected override void OnRemoved(Node parent)
    {
        switch (parent)
        {
            case Element parentElement:
                this.Layout.HandleTargetRemoved(parentElement);
                break;

            case ShadowTree shadowTree:
                this.Layout.HandleTargetRemoved(shadowTree.Host);
                break;
        }
    }
}
