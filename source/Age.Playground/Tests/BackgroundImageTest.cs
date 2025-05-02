using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.Playground.Tests;

public class BackgroundImageTest : Element
{
    public override string NodeName => nameof(BackgroundImageTest);

    public static void Setup(Canvas canvas)
    {
        var uri = Path.Join(AppContext.BaseDirectory, "Assets", "Textures", "Cat.jpg");

        var border = 10u;

        canvas.Children =
        [
            new FlexBox
            {
                Style = new Style
                {
                    BackgroundColor = Color.White,
                    BackgroundImage = new()
                    {
                        Uri       = uri,
                        Size      = ImageSize.Fit(),
                        Transform = Transform2D.CreateRotated(Angle.DegreesToRadians(45f))
                    },
                    Size            = new((Pixel)100, (Pixel)200),
                    Border          = new(border, 0, Color.Red)
                }
            },
            new FlexBox
            {
                Style = new Style
                {
                    BackgroundColor = Color.White,
                    BackgroundImage = new()
                    {
                        Uri       = uri,
                        Size      = ImageSize.KeepAspect(),
                        Transform = Transform2D.CreateRotated(Angle.DegreesToRadians(45f))
                    },
                    Size            = new((Pixel)100, (Pixel)200),
                    Border          = new(border, 0, Color.Green)
                }
            },
            new FlexBox
            {
                Style = new Style
                {
                    BackgroundColor = Color.White,
                    BackgroundImage = new() { Uri = uri, Size = ImageSize.Size((Pixel)50), Repeat = ImageRepeat.NoRepeat },
                    Size            = new((Pixel)100, (Pixel)200),
                    Border          = new(border, 0, Color.Blue)
                }
            },
            new FlexBox
            {
                Style = new Style
                {
                    //Margin          = new((Pixel)50),
                    BackgroundColor = Color.White,
                    BackgroundImage = new()
                    {
                        Uri       = uri,
                        Size      = ImageSize.Size((Pixel)75),
                        Repeat    = ImageRepeat.Repeat,
                    },
                    Transform = Transform2D.CreateRotated(Angle.DegreesToRadians(45f)),
                    Size      = new((Pixel)150, (Pixel)200),
                    Border    = new(border, 0, Color.Margenta)
                }
            },
            new FlexBox
            {
                Style = new Style
                {
                    //Margin          = new((Pixel)50),
                    BackgroundColor = Color.White,
                    BackgroundImage = new()
                    {
                        Uri       = uri,
                        Size      = ImageSize.Size((Pixel)75),
                        Repeat    = ImageRepeat.RepeatX,
                        Transform = Transform2D.CreateRotated(Angle.DegreesToRadians(-45f))
                    },
                    Size            = new((Pixel)150, (Pixel)200),
                    Border          = new(border, 0, Color.Yellow)
                }
            },
            new FlexBox
            {
                Style = new Style
                {
                    //Margin          = new((Pixel)50),
                    BackgroundColor = Color.White,
                    BackgroundImage = new()
                    {
                        Uri       = uri,
                        Size      = ImageSize.Size((Pixel)75),
                        Repeat    = ImageRepeat.RepeatY,
                        Transform = Transform2D.CreateRotated(Angle.DegreesToRadians(-45f))
                    },
                    Size            = new((Pixel)150, (Pixel)200),
                    Border          = new(border, 0, Color.Cyan)
                }
            },
            new FlexBox
            {
                Style = new Style
                {
                    //Margin          = new((Pixel)50),
                    BackgroundColor = Color.White,
                    BackgroundImage = new()
                    {
                        Uri       = uri,
                        Size      = ImageSize.Size((Pixel)75),
                        Repeat    = ImageRepeat.NoRepeat,
                        Transform = Transform2D.CreateRotated(Angle.DegreesToRadians(45f))
                    },
                    Size            = new((Pixel)150, (Pixel)200),
                    Border          = new(border, 0, Color.Cyan)
                }
            }
        ];
    }
}
