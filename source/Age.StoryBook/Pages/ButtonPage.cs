using Age.Components;
using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.StoryBook.Pages;

public class ButtonPage : Page
{
    public override string NodeName => nameof(ButtonPage);
    public override string Title    => nameof(Button);

    public ButtonPage()
    {
        var buttonStyle = new Style
        {
            Margin = new(Unit.Px(2), null),
        };

        this.Children =
        [
            new FlexBox
            {
                Style = new()
                {
                    ContentJustification = ContentJustification.SpaceAround,
                    Size                 = new(Unit.Pc(100), null),
                    Margin               = new(Unit.Px(10)),
                },
                Children =
                [
                    new Button
                    {
                        InnerText = "Flat",
                        Variant   = ButtonVariant.Flat,
                        Style     = buttonStyle,
                    },
                    new Button
                    {
                        InnerText = "Outlined",
                        Variant   = ButtonVariant.Outlined,
                        Style     = buttonStyle,
                    },
                    new Button
                    {
                        InnerText = "Text",
                        Variant   = ButtonVariant.Text,
                        Style     = buttonStyle,
                    }
                ]
            }
        ];
    }
}
