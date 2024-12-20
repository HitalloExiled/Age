using Age.Components;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public static class ButtonTest
{
    public static void Setup(Canvas canvas)
    {
        var container = new FlexBox
        {
            Style = new()
            {
                ContentJustification = ContentJustificationKind.SpaceAround,
                Size                 = new((Percentage)100, null),
                Margin               = new((Pixel)10),
            }
        };
        var button1 = new Button() { Text = "Flat",     Variant = ButtonVariant.Flat };
        var button2 = new Button() { Text = "Outlined", Variant = ButtonVariant.Outlined, Style = new() { Margin = new((Pixel)4, null) } };
        var button3 = new Button() { Text = "Text",     Variant = ButtonVariant.Text,     Style = new() { Margin = new((Pixel)4, null) } };

        canvas.AppendChild(container);
            container.AppendChild(button1);
            container.AppendChild(button2);
            container.AppendChild(button3);
    }
}
