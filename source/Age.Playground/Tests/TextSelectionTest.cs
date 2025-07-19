using Age.Elements;
using Age.Numerics;
using Age.Platforms.Display;
using Age.Styling;

namespace Age.Playground.Tests;

public static class TextSelectionTest
{
    public static void Setup(Canvas canvas)
    {
        var hello = new FlexBox
        {
            InnerText = "Hello",
            Style     = new()
            {
                Color      = Color.White,
                FontSize   = 100,
                // Transform  = Transform2D.CreateTranslated(20, -20),
                //Transform  = Transform2D.CreateScaled(0.5f) * Transform2D.CreateRotated(Angle.DegreesToRadians(45f)) * Transform2D.CreateTranslated(200, -20),
                Transforms =
                [
                    TransformOp.Translate(Unit.Px(200), Unit.Px(-20)),
                    TransformOp.Rotate(Angle.DegreesToRadians(45f)),
                    TransformOp.Scale(0.5f),
                ],
                // FontFamily = "Cascadia Code",
                Border     = new(20, 0, Color.Red),
                FontFamily = "Consolas",
            },
            StyleSheet = new()
            {
                Hovered = new()
                {
                    Cursor = Cursor.Hand,
                }
            }
        };

        var world = new FlexBox
        {
            InnerText = "World",
            Style     = new()
            {
                Color      = Color.White,
                FontSize   = 100,
                // Transform  = Transform2D.CreateTranslated(20, -20),
                //Transform  = Transform2D.CreateRotated(Angle.DegreesToRadians(-45f)) * Transform2D.CreateTranslated(0, -100),
                Transforms =
                [
                    TransformOp.Translate(new(Unit.Px(0), Unit.Px(-100))),
                    TransformOp.Rotate(Angle.DegreesToRadians(-45f)),
                    TransformOp.Scale(0.5f),
                ],
                // FontFamily = "Cascadia Code",
                Border     = new(20, 0, Color.Red),
                FontFamily = "Consolas",
            },
            StyleSheet = new()
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
