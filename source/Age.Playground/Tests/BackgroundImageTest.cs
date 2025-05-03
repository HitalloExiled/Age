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
                        Position  = new(Unit.Pc(50)),
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
                        Position  = new(Unit.Pc(0), Unit.Pc(50)),
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
                    BackgroundImage = new()
                    {
                        Uri    = uri,
                        Size   = ImageSize.Size((Pixel)50),
                        Repeat = ImageRepeat.NoRepeat,
                        Position = new(Unit.Px(25)),
                    },
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
                        Position  = new(Unit.Pc(0), Unit.Pc(50)),
                    },
                    TransformOrigin = new(Unit.Pc(100), Unit.Pc(100)),
                    Transform       = new()
                    {
                        Rotation = Angle.DegreesToRadians(-5f),
                        Position = new(Unit.Pc(0), Unit.Pc(50)),
                        Scale    = new(1.25f, 0.75f),
                    },
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
                        Position  = new(Unit.Pc(50)),
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
                        Position  = new(Unit.Pc(50)),
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
                        Position  = new(Unit.Pc(50)),
                    },
                    Size            = new((Pixel)150, (Pixel)200),
                    Border          = new(border, 0, Color.Cyan)
                }
            }
        ];
    }
}
