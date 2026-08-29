using Age.Elements;
using Age.Numerics;
using Age.Scenes;
using Age.Styling;
using System.Diagnostics;

namespace Age.Playground.Tests.Scenes;

public static class SubViewportTest
{
    private static DemoScene? scene;

    public static void Setup(Canvas canvas)
    {
        const uint BORDER_SIZE = 2;

        scene?.Dispose();
        scene = new DemoScene();

        Debug.Assert(canvas.Scene != null);

        canvas.Scene.AppendChild(scene);

        var subViewportFree = new SubViewport(new(608)) { Name = "Free", Scene = scene, Filter = SceneFilter.World3D, Camera3D = scene.FreeCamera };
        var subViewportX    = new SubViewport(new(200)) { Name = "X",    Scene = scene, Filter = SceneFilter.World3D, Camera3D = scene.RedCamera };
        var subViewportY    = new SubViewport(new(200)) { Name = "Y",    Scene = scene, Filter = SceneFilter.World3D, Camera3D = scene.GreenCamera };
        var subViewportZ    = new SubViewport(new(200)) { Name = "Z",    Scene = scene, Filter = SceneFilter.World3D, Camera3D = scene.BlueCamera };

        canvas.Scene.AppendChildren([
            subViewportFree,
            subViewportX,
            subViewportY,
            subViewportZ,
        ]);

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
