using Age.Elements;
using Age.Numerics;
using Age.Platforms.Display;

namespace Age.Playground.Tests;

public class TextSelectionTest
{
    public static void Setup(Canvas canvas)
    {
        var hello = new FlexBox
        {
            Text = "Hello",
            Style = new()
            {
                Color      = Color.White,
                FontSize   = 100,
                // Transform  = Transform2D.CreateTranslated(20, -20),
                Transform  = Transform2D.CreateScaled(0.5f) * Transform2D.CreateRotated(Angle.DegreesToRadians(45f)) * Transform2D.CreateTranslated(200, -20),
                // FontFamily = "Cascadia Code",
                Border     = new(20, 0, Color.Red),
                FontFamily = "Consolas",
            },
            States = new()
            {
                Hovered = new()
                {
                    Cursor = Cursor.Hand,
                }
            }
        };

        var world = new FlexBox
        {
            Text = "World",
            Style = new()
            {
                Color      = Color.White,
                FontSize   = 100,
                // Transform  = Transform2D.CreateTranslated(20, -20),
                Transform  = Transform2D.CreateRotated(Angle.DegreesToRadians(-45f)) * Transform2D.CreateTranslated(0, -100),
                // FontFamily = "Cascadia Code",
                Border     = new(20, 0, Color.Red),
                FontFamily = "Consolas",
            },
            States = new()
            {
                Hovered = new()
                {
                    Cursor = Cursor.Busy,
                },
                Active = new()
                {
                    Cursor = Cursor.Move,
                    Border = new(20, 0, Color.White),
                }
            }
        };

        world.Focus();

        canvas.AppendChild(hello);
        canvas.AppendChild(world);
    }
}
