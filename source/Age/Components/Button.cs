using Age.Elements;
using Age.Themes;

namespace Age.Components;

public class Button : Element
{
    public override string NodeName { get; } = nameof(Button);

    public Button()
    {
        this.IsFocusable = true;
        this.States      = Theme.Current.Button;
    }
}
