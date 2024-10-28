using Age.Elements;
using Age.Numerics;

namespace Age.Editor.Tests;

public class TextSelectionTest
{
    public static void Setup(Canvas canvas)
    {
        var text = new FlexBox
        {
            Text =
                """
                Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                Integer sed vestibulum lectus.
                Curabitur vel vestibulum massa.
                Ut non nunc ornare, porttitor augue eu, rutrum dui.
                Nunc fermentum metus vitae orci tristique, ut cursus nunc sollicitudin.
                Aenean quis faucibus sem. Nulla non magna non justo placerat imperdiet eu a orci.
                Maecenas sit amet dolor magna.
                """,
            Style = new()
            {
                Color      = Color.White,
                // FontSize   = 100,
                Transform  = Transform2D.CreateTranslated(20, -20),
                // FontFamily = "Cascadia Code",
                Border     = new(20, 0, Color.Red),
                FontFamily = "Consolas",
            }
        };

        canvas.AppendChild(text);
    }
}
