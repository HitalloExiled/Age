using Age.Components;
using Age.Elements;

namespace Age.Playground.Tests.Components;

public static class TextBoxTest
{
    public static void Setup(Canvas canvas) =>
        canvas.Children =
        [
            new TextBox
            {
                Value = "O😹O😁",
                Style = new()
                {
                    FontFamily = "Segoe UI Emoji",
                }
            },
            new TextBox
            {
                Value =
                    """
                    O😹O😁
                    O😹O😁
                    😹O😁O
                    O😹O😁
                    .......
                    """,
                Multiline = true,
                Style = new()
                {
                    FontFamily = "Segoe UI Emoji",
                }
            }
        ];
}
