using Age.Elements;
using Age.Numerics;
using Age.Scenes;
using Age.Styling;
using System.Diagnostics;

namespace Age.Playground.Tests.Scene;

public static class SubViewportTest
{
    public static void Setup(Canvas canvas)
    {
        const uint BORDER_SIZE = 2;

        var scene = new DemoScene();

        Debug.Assert(canvas.Scene?.Viewport != null);

        canvas.Scene.Viewport.Scene3D?.Dispose();
        canvas.Scene.Viewport.Scene3D = scene;

        var subViewportFree = new SubViewport(new(608)) { Name = "Free", Camera3D = scene.FreeCamera,  Scene3D = scene };
        var subViewportX    = new SubViewport(new(200)) { Name = "X",    Camera3D = scene.RedCamera,   Scene3D = scene };
        var subViewportY    = new SubViewport(new(200)) { Name = "Y",    Camera3D = scene.GreenCamera, Scene3D = scene };
        var subViewportZ    = new SubViewport(new(200)) { Name = "Z",    Camera3D = scene.BlueCamera,  Scene3D = scene };

        canvas.Scene.Viewport.Scene2D?.Dispose();
        canvas.Scene.Viewport.Scene2D = new Scene2D { Children = [subViewportFree, subViewportX, subViewportY, subViewportZ] };

        var root = new FlexBox
        {
            Name  = "Root",
            Style =
            {
                Size   = new(Unit.Pc(100)),
                Border = new(BORDER_SIZE, default, Color.Margenta),
            },
            Children =
            [
                new FlexBox()
                {
                    Name  = "VStack",
                    Style =
                    {
                        StackDirection = StackDirection.Vertical,
                        Size           = new(Unit.Pc(100)),
                        Border         = new(BORDER_SIZE, default, Color.Yellow),
                    },
                    Children =
                    [
                        new FlexBox
                        {
                            Name  = "Header",
                            Style =
                            {
                                Size   = new(Unit.Pc(100), null),
                                Border = new(BORDER_SIZE, default, Color.Red),
                            },
                            Children = [new FrameStatus()]
                        },
                        new FlexBox
                        {
                            Name  = "Viewports",
                            Style =
                            {
                                Alignment = Alignment.Center,
                            },
                            Children =
                            [
                                new FlexBox
                                {
                                    Style =
                                    {
                                        Border = new(BORDER_SIZE, default, Color.Margenta),
                                    },
                                    Children = [new EmbeddedViewport(subViewportFree)]
                                },
                                new FlexBox
                                {
                                    Style =
                                    {
                                        StackDirection = StackDirection.Vertical,
                                    },
                                    Children =
                                    [
                                        new FlexBox
                                        {
                                            Style =
                                            {
                                                Border = new(BORDER_SIZE, default, Color.Red),
                                            },
                                            Children = [new EmbeddedViewport(subViewportX)]
                                        },
                                        new FlexBox
                                        {
                                            Style =
                                            {
                                                Border = new(BORDER_SIZE, default, Color.Green),
                                            },
                                            Children = [new EmbeddedViewport(subViewportY)]
                                        },
                                        new FlexBox
                                        {
                                            Style =
                                            {
                                                Border = new(BORDER_SIZE, default, Color.Blue),
                                            },
                                            Children = [new EmbeddedViewport(subViewportZ)]
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        canvas.AppendChild(root);
    }
}
