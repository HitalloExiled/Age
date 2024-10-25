using Age.Elements;
using Age.Numerics;

namespace Age.Editor.Tests;

public class TextSelectionTest
{
    public static void Setup(Canvas canvas)
    {
        var text = new FlexBox
        {
            Text = "( {[ÁÃẤçqxX]})",
            Style = new()
            {
                Color      = Color.White,
                FontSize   = 100,
                Transform  = Transform2D.CreateTranslated(0, -10),
                // FontFamily = "Cascadia Code",
                FontFamily = "Consolas",
            }
        };

        canvas.AppendChild(text);
    }
}
