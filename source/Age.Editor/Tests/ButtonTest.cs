using Age.Components;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public static class ButtonTest
{
    public static void Setup(Canvas canvas)
    {
        var button1 = new Button() { Text = "Click Me!!!" };
        var button2 = new Button() { Text = "Click Me Too!!!", Style = new() { Margin = new((Pixel)4, null) } };

        canvas.AppendChild(button1);
        canvas.AppendChild(button2);
    }
}
