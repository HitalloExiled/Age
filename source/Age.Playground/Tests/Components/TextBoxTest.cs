using Age.Components;
using Age.Elements;

namespace Age.Playground.Tests.Components;

public static class TextBoxTest
{
    public static void Setup(Canvas canvas)
    {
        var textBox = new TextBox
        {
            Value = "O😹O😁",
            Style = new()
            {
                FontFamily = "Segoe UI Emoji",
            }
        };

        canvas.Children = [textBox];
    }
}
