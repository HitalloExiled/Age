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

        FlexBox img1;
        FlexBox img2;
        FlexBox img3;

        canvas.Children =
        [
            img1 = new FlexBox
            {
                Style = new Style
                {
                    BackgroundColor = Color.White,
                    BackgroundImage = new(uri, ImageSize.Fit()),
                    Size            = new((Pixel)100, (Pixel)200),
                    Border          = new(border, 0, Color.Red)
                }
            },
            img2 = new FlexBox
            {
                Style = new Style
                {
                    BackgroundColor = Color.White,
                    BackgroundImage = new(uri, ImageSize.KeepAspect()),
                    Size            = new((Pixel)100, (Pixel)200),
                    Border          = new(border, 0, Color.Green)
                }
            },
            img3 = new FlexBox
            {
                Style = new Style
                {
                    BackgroundColor = Color.White,
                    BackgroundImage = new(uri, ImageSize.Size((Pixel)50), ImageRepeat.NoRepeat),
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
                    BackgroundImage = new(uri, ImageSize.Size((Pixel)75), ImageRepeat.Repeat),
                    Size            = new((Pixel)150, (Pixel)200),
                    Border          = new(border, 0, Color.Margenta)
                }
            },
            new FlexBox
            {
                Style = new Style
                {
                    //Margin          = new((Pixel)50),
                    BackgroundColor = Color.White,
                    BackgroundImage = new(uri, ImageSize.Size((Pixel)75), ImageRepeat.RepeatX),
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
                    BackgroundImage = new(uri, ImageSize.Size((Pixel)75), ImageRepeat.RepeatY),
                    Size            = new((Pixel)150, (Pixel)200),
                    Border          = new(border, 0, Color.Cyan)
                }
            }
        ];

        void onClick(in MouseEvent mouseEvent) =>
            mouseEvent.Target.Style.BackgroundImage = mouseEvent.Target.Style.BackgroundImage == null
                ? new(uri, ImageSize.Fit())
                : null;

        img1.Clicked += onClick;
        img2.Clicked += onClick;
        img3.Clicked += onClick;
    }
}
