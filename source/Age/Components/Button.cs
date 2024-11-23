using Age.Elements;
using Age.Themes;

namespace Age.Components;

public enum ButtonVariant
{
    Flat,
    Outlined,
    Text,
}

public class Button : Element
{
    private ButtonVariant variant;

    public override string NodeName { get; } = nameof(Button);

    public ButtonVariant Variant
    {
        get => this.variant;
        set
        {
            if (this.variant != value)
            {
                this.States = value switch
                {
                   ButtonVariant.Flat      => Theme.Current.Button.Flat,
                   ButtonVariant.Outlined  => Theme.Current.Button.Outlined,
                   ButtonVariant.Text or _ => Theme.Current.Button.Text,
                };

                this.variant = value;
            }
        }
    }

    public Button()
    {
        this.IsFocusable = true;
        this.States      = Theme.Current.Button.Flat;
    }
}
