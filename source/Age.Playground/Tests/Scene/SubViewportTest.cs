using Age.Elements;
using Age.Numerics;
using Age.Scenes;
using Age.Styling;

namespace Age.Playground.Tests.Scene;

public static class SubViewportTest
{
    public static void Setup(Canvas canvas)
    {
        const uint BORDER_SIZE = 2;

        canvas.Scene!.Viewport!.Scene3D?.Dispose();

        var scene = new DemoScene();

        canvas.Scene!.Viewport!.Scene3D = scene;

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
                                Border    = new(BORDER_SIZE, default, Color.Cyan),
                            },
                            Children =
                            [
                                new FlexBox
                                {
                                    Style =
                                    {
                                        Size   = new(600),
                                        Border = new(BORDER_SIZE, default, Color.Margenta),
                                    },
                                    Children = [new SubViewport(new(600)) { Name = "Free", Camera3D = scene.FreeCamera, Scene3D = scene }]
                                },
                                new FlexBox
                                {
                                    Style =
                                    {
                                        StackDirection = StackDirection.Vertical,
                                        Border         = new(BORDER_SIZE, default, Color.Cyan),
                                    },
                                    Children =
                                    [
                                        new FlexBox
                                        {
                                            Style =
                                            {
                                                Size   = new(200),
                                                Border = new(BORDER_SIZE, default, Color.Red)
                                            },
                                            Children = [new SubViewport(new(200)) { Name = "X", Camera3D = scene.RedCamera, Scene3D = scene }]
                                        },
                                        new FlexBox
                                        {
                                            Style =
                                            {
                                                Size   = new(200),
                                                Border = new(BORDER_SIZE, default, Color.Green)
                                            },
                                            Children = [new SubViewport(new(200)) { Name = "Y", Camera3D = scene.GreenCamera, Scene3D = scene }]
                                        },
                                        new FlexBox
                                        {
                                            Style =
                                            {
                                                Size   = new(200),
                                                Border = new(BORDER_SIZE, default, Color.Blue)
                                            },
                                            Children = [new SubViewport(new(200)) { Name = "Z", Camera3D = scene.BlueCamera, Scene3D = scene }]
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
