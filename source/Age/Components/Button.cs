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
    public override string NodeName => nameof(Button);

    public ButtonVariant Variant
    {
        get;
        set
        {
            if (field != value)
            {
                this.StyleSheet = value switch
                {
                   ButtonVariant.Flat      => Theme.Current.Button.Flat,
                   ButtonVariant.Outlined  => Theme.Current.Button.Outlined,
                   ButtonVariant.Text or _ => Theme.Current.Button.Text,
                };

                field = value;
            }
        }
    }

    public Button()
    {
        this.IsFocusable = true;
        this.StyleSheet  = Theme.Current.Button.Flat;
    }
}
