using Age.Components;
using Age.Elements;
using Age.Styling;

namespace Age.StoryBook;

public class ButtonPage : Page
{
    public override string NodeName { get; } = nameof(ButtonPage);
    public override string Title    { get; } = "Button";

    public ButtonPage()
    {
        var buttonStyle = new Style
        {
            Margin = new((Pixel)2, null),
        };

        var container = new FlexBox
        {
            Style = new()
            {
                ContentJustification = ContentJustificationKind.SpaceAround,
                Size                 = new((Percentage)100, null),
                Margin               = new((Pixel)10),
            },
            Children =
            [
                new Button
                {
                    Text    = "Flat",
                    Variant = ButtonVariant.Flat,
                    Style   = buttonStyle,
                },
                new Button
                {
                    Text    = "Outlined",
                    Variant = ButtonVariant.Outlined,
                    Style   = buttonStyle,
                },
                new Button
                {
                    Text    = "Text",
                    Variant = ButtonVariant.Text,
                    Style   = buttonStyle,
                }
            ]
        };

        this.AppendChild(container);
    }
}
