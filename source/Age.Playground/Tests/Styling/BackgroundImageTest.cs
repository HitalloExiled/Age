using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.Playground.Tests.Styling;

public class BackgroundImageTest : Element
{
    public override string NodeName => nameof(BackgroundImageTest);

    public static void Setup(Canvas canvas)
    {
        var uri = Path.Join(AppContext.BaseDirectory, "Assets", "Textures", "Cat.jpg");

        const uint BORDER = 10u;

        canvas.Children =
        [
            new Text("Click me..."),
            new FlexBox
            {
               Style = new Style
               {
                   BackgroundColor = Color.White,
                   BackgroundImage = new(uri)
                   {
                       Size     = ImageSize.Fit(),
                       Position = new(Unit.Pc(50)),
                   },
                   Size   = new(Unit.Px(100), Unit.Px(200)),
                   Border = new(BORDER, 0, Color.Red)
               }
            },
            new FlexBox
            {
               Style = new Style
               {
                   BackgroundColor = Color.White,
                   BackgroundImage = new(uri)
                   {
                       Size     = ImageSize.KeepAspect(),
                       Position = new(Unit.Pc(0), Unit.Pc(50)),
                   },
                   Size   = new(Unit.Px(100), Unit.Px(200)),
                   Border = new(BORDER, 0, Color.Green)
               }
            },
            new FlexBox
            {
               Style = new Style
               {
                   BackgroundColor = Color.White,
                   BackgroundImage = new(uri)
                   {
                       Size     = ImageSize.Size(Unit.Px(50)),
                       Repeat   = ImageRepeat.NoRepeat,
                       Position = new(Unit.Px(25)),
                   },
                   Size            = new(Unit.Px(100), Unit.Px(200)),
                   Border          = new(BORDER, 0, Color.Blue)
               }
            },
            new FlexBox
            {
                Style = new Style
                {
                    Size = new(Unit.Px(200), Unit.Px(200)),
                    Border = new(1, 0, Color.Red),
                },
                Children =
                [
                    new FlexBox
                    {
                        Style = new Style
                        {
                            //Margin          = new(Unit.Px(50)),

                            BackgroundColor = Color.White,
                            BackgroundImage = new(uri)
                            {
                                Size     = ImageSize.Size(Unit.Px(100)),
                                Repeat   = ImageRepeat.Repeat,
                                Position = new(Unit.Pc(50)),
                            },
                            TransformOrigin = new(Unit.Pc(0), default),
                            Transforms =
                            [
                                TransformOp.Translate(default, Unit.Pc(50)),
                                TransformOp.Rotate(Angle.DegreesToRadians(-5f)),
                                TransformOp.Scale(0.5f, 1),
                                TransformOp.Matrix(1.2f, 0.5f, 0.5f, 1.2f, 10, 20),
                            ],
                            Size   = new(Unit.Px(200), Unit.Px(200)),
                            //Border = new(0, 0, Color.Margenta)
                        }
                    },
                ]
            },
            new FlexBox
            {
               Style = new Style
               {
                   //Margin          = new(Unit.Px(50)),
                   BackgroundColor = Color.White,
                   BackgroundImage = new(uri)
                   {
                       Size     = ImageSize.Size(Unit.Px(75)),
                       Repeat   = ImageRepeat.RepeatX,
                       Position = new(Unit.Pc(50)),
                   },
                   Size            = new(Unit.Px(150), Unit.Px(200)),
                   Border          = new(BORDER, 0, Color.Yellow)
               }
            },
            new FlexBox
            {
               Style = new Style
               {
                   //Margin          = new(Unit.Px(50)),
                   BackgroundColor = Color.White,
                   BackgroundImage = new(uri)
                   {
                       Size     = ImageSize.Size(Unit.Px(75)),
                       Repeat   = ImageRepeat.RepeatY,
                       Position = new(Unit.Pc(50)),
                   },
                   Size            = new(Unit.Px(150), Unit.Px(200)),
                   Border          = new(BORDER, 0, Color.Cyan)
               }
            },
            new FlexBox
            {
               Style = new Style
               {
                   //Margin          = new(Unit.Px(50)),
                   BackgroundColor = Color.White,
                   BackgroundImage = new(uri)
                   {
                       Size     = ImageSize.Size(Unit.Px(75)),
                       Repeat   = ImageRepeat.NoRepeat,
                       Position = new(Unit.Pc(50)),
                   },
                   Size            = new(Unit.Px(150), Unit.Px(200)),
                   Border          = new(BORDER, 0, Color.Cyan)
               }
            }
        ];
    }
}
