using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Elements;
using Age.Scenes;
using Age.Tests.Age.Acessors;
using Age.Tests.Age.Fixtures;

namespace Age.Tests.Age.Scenes;

#pragma warning disable CS9113

file static class Entensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "nodes")]
    private static extern ref List<Renderable> GetNodes(SceneGraphCache sceneGraphCache);

    extension<T>(T[] nodes) where T : Node
    {
        public T[] IgnoreEmpty() =>
            [.. nodes.Where(static x => x is not Empty)];
    }

    extension(SceneGraphCache sceneGraphCache)
    {
        public List<Renderable> NodesList => GetNodes(sceneGraphCache);
    }
}

[Collection("GPU")]
public partial class SceneGraphCacheTest(GpuFixture _)
{
    private sealed record ComponentCommand : Command2D;
    private sealed record SpriteCommand : Command2D;
    private sealed record ModelCommand : Command3D;

    private static void Emit(NodeRange[] expected, NodeRange[] actual, [CallerFilePath] string? callerFilePath = null)
    {
        var rawExpected = string.Join('\n', expected);
        var rawActual   = string.Join('\n', actual);

        var folder = Path.GetDirectoryName(callerFilePath);

        File.WriteAllText(Path.Join(folder, "expected.txt"), rawExpected);
        File.WriteAllText(Path.Join(folder, "actual.txt"), rawActual);
    }

    private static NodeRange ToNodeRange(Renderable node)
    {
        var subtree  = node.SubtreeRange;

        CommandRange commandRange = default;

        if (node is Renderable<Command2D> renderable2D)
        {
            commandRange = renderable2D.CommandRange;
        }
        else if (node is Renderable<Command3D> renderable3D)
        {
            commandRange = renderable3D.CommandRange;
        }

        return new(node, subtree, commandRange);
    }

    [Fact]
    public void BuildOnce_BuildsSceneGraph()
    {
        var window = Window.CreateMock();

        var cache = RenderTreeAccessor.GetSceneGraphCache(window.RenderTree);

        window.Scene = new Scene
        {
            Name    = "$",
            World3D = new World3D
            {
                Name = "$.1",
            },
            World2D = new World2D
            {
                Name     = "$.2",
                Children =
                [
                    TreeFactory.Linear<Sprite, Command2D, SpriteCommand>(2, 2, 2, -1, "$.2.1"),
                ]
            },
            Canvas = new Canvas
            {
                Name     = "$.3",
                Children =
                [
                    TreeFactory.Linear<SealedComponent, Command2D, ComponentCommand>(static name => new(name), 1, 1, 3, 1, "$.3.1"),
                    TreeFactory.Linear<Component, Command2D, ComponentCommand>(2, 2, 2, -1, "$.3.3"),
                ],
            },
            Children =
            [
                new SubViewport(new(400))
                {
                    Name  = "$.4",
                    Scene = new()
                    {
                        Name    = "$.4.1",
                        World3D = new()
                        {
                            Name     = "$.4.1.1",
                            Children =
                            [
                                TreeFactory.Linear<Model, Command3D, MeshCommand>(2, 2, 2, -1, "$.4.1.1.1"),
                            ],
                        }
                    }
                },
            ],
        };

        cache.InvalidatedSubTree(window);
        cache.Build();

        var flat     = TreeFactory.Flatten(window).IgnoreEmpty();
        var expected = new NodeRange[49]
        {
            #region Window
            new(flat[0], new(0, 49)),
                #region Scene
                new(flat[1], new(1, 49)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 38)),
                        new(flat[3], new(3, 4)), // World3D

                        #region World2D
                        new(flat[4], new(4, 12)),
                            #region Sprite
                            new(flat[5], new(5, 12), new(0, 2, 14)),
                                #region Sprite
                                new(flat[6], new(6, 9), new(2, 4, 8)),
                                    new(flat[7], new(7, 8), new(4, 6)), // Sprite
                                    new(flat[8], new(8, 9), new(6, 8)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[9], new(9, 12), new(8, 10, 14)),
                                    new(flat[10], new(10, 11), new(10, 12)), // Sprite
                                    new(flat[11], new(11, 12), new(12, 14)), // Sprite
                                #endregion Sprite
                            #endregion Sprite
                        #endregion World2D

                        #region Canvas
                        new(flat[12], new(12, 38), new(0, 1, 130)),
                            #region SealedComponent
                            new(flat[13], new(13, 31), new(1, 2, 106, 109)),
                                #region #ShadowRoot
                                new(flat[14], new(14, 22), new(2, 3, 52)),
                                    #region Component
                                    new(flat[15], new(15, 19), new(3, 4, 25, 31)),
                                        new(flat[16], new(16, 17), new(4, 5, 5, 11)), // Component
                                        new(flat[17], new(17, 18), new(11, 12, 12, 18)), // Component
                                        new(flat[18], new(18, 19), new(18, 19, 19, 25)), // Component
                                    #endregion Component

                                    #region Component
                                    new(flat[19], new(19, 22), new(31, 32, 46, 52)),
                                        new(flat[20], new(20, 21), new(32, 33, 33, 39)), // Component
                                        new(flat[21], new(21, 22), new(39, 40, 40, 46)), // Component
                                    #endregion Component
                                #endregion #ShadowRoot

                                #region SealedComponent
                                new(flat[22], new(22, 31), new(52, 53, 103, 106)),
                                    #region #ShadowRoot
                                    new(flat[23], new(23, 31), new(53, 54, 103)),
                                        #region Component
                                        new(flat[24], new(24, 28), new(54, 55, 76, 82)),
                                            new(flat[25], new(25, 26), new(55, 56, 56, 62)), // Component
                                            new(flat[26], new(26, 27), new(62, 63, 63, 69)), // Component
                                            new(flat[27], new(27, 28), new(69, 70, 70, 76)), // Component
                                        #endregion Component

                                        #region Component
                                        new(flat[28], new(28, 31), new(82, 83, 97, 103)),
                                            new(flat[29], new(29, 30), new(83, 84, 84, 90)), // Component
                                            new(flat[30], new(30, 31), new(90, 91, 91, 97)), // Component
                                        #endregion Component
                                    #endregion #ShadowRoot
                                #endregion SealedComponent
                            #endregion SealedComponent

                            #region Component
                            new(flat[31], new(31, 38), new(109, 110, 128, 130)),
                                #region Component
                                new(flat[32], new(32, 35), new(110, 111, 117, 119)),
                                    new(flat[33], new(33, 34), new(111, 112, 112, 114)), // Component
                                    new(flat[34], new(34, 35), new(114, 115, 115, 117)), // Component
                                #endregion Component

                                #region Component
                                new(flat[35], new(35, 38), new(119, 120, 126, 128)),
                                    new(flat[36], new(36, 37), new(120, 121, 121, 123)), // Component
                                    new(flat[37], new(37, 38), new(123, 124, 124, 126)), // Component
                                #endregion Component
                            #endregion Component
                        #endregion Canvas

                        #region SubViewport
                        new(flat[38], new(38, 49)),
                            #region Scene
                            new(flat[39], new(39, 49)),
                                #region #ShadowRoot
                                new(flat[40], new(40, 49)),
                                    #region World3D
                                    new(flat[41], new(41, 49)),
                                        #region Model
                                        new(flat[42], new(42, 49), new(0, 2, 14)),
                                            #region Model
                                            new(flat[43], new(43, 46), new(2, 4, 8)),
                                                new(flat[44], new(44, 45), new(4, 6)), // Model
                                                new(flat[45], new(45, 46), new(6, 8)), // Model
                                            #endregion Model

                                            #region Model
                                            new(flat[46], new(46, 49), new(8, 10, 14)),
                                                new(flat[47], new(47, 48), new(10, 12)), // Model
                                                new(flat[48], new(48, 49), new(12, 14)), // Model
                                            #endregion Model
                                        #endregion Model
                                    #endregion World3D
                                #endregion #ShadowRoot
                            #endregion Scene
                        #endregion SubViewport
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        };

        var actual = cache.NodesList.Select(ToNodeRange).ToArray();

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        flat[13].Detach();

        cache.Build();

        flat = TreeFactory.Flatten(window).IgnoreEmpty();

        expected =
        [
            #region Window
            new(flat[0], new(0, 31)),
                #region Scene
                new(flat[1], new(1, 31)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 20)),
                        new(flat[3], new(3, 4)), // World3D

                        #region World2D
                        new(flat[4], new(4, 12)),
                            #region Sprite
                            new(flat[5], new(5, 12), new(0, 2, 14)),
                                #region Sprite
                                new(flat[6], new(6, 9), new(2, 4, 8)),
                                    new(flat[7], new(7, 8), new(4, 6)), // Sprite
                                    new(flat[8], new(8, 9), new(6, 8)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[9], new(9, 12), new(8, 10, 14)),
                                    new(flat[10], new(10, 11), new(10, 12)),   // Sprite
                                    new(flat[11], new(11, 12), new(12, 14)), // Sprite
                                #endregion Sprite
                            #endregion Sprite
                        #endregion World2D

                        #region Canvas
                        new(flat[12], new(12, 20), new(0, 1, 22)),
                            #region Component
                            new(flat[13], new(13, 20), new(1, 2, 20, 22)),
                                #region Component
                                new(flat[14], new(14, 17), new(2, 3, 9, 11)),
                                    new(flat[15], new(15, 16), new(3, 4, 4, 6)), // Component
                                    new(flat[16], new(16, 17), new(6, 7, 7, 9)), // Component
                                #endregion Component

                                #region Component
                                new(flat[17], new(17, 20), new(11, 12, 18, 20)),
                                    new(flat[18], new(18, 19), new(12, 13, 13, 15)), // Component
                                    new(flat[19], new(19, 20), new(15, 16, 16, 18)), // Component
                                #endregion Component
                            #endregion Component
                        #endregion Canvas

                        #region SubViewport
                        new(flat[20], new(20, 31)),
                            #region Scene
                            new(flat[21], new(21, 31)),
                                #region #ShadowRoot
                                new(flat[22], new(22, 31)),
                                    #region World3D
                                    new(flat[23], new(23, 31)),
                                        #region Model
                                        new(flat[24], new(24, 31), new(0, 2, 14)),
                                            #region Model
                                            new(flat[25], new(25, 28), new(2, 4, 8)),
                                                new(flat[26], new(26, 27), new(4, 6)), // Model
                                                new(flat[27], new(27, 28), new(6, 8)), // Model
                                            #endregion Model

                                            #region Model
                                            new(flat[28], new(28, 31), new(8, 10, 14)),
                                                new(flat[29], new(29, 30), new(10, 12)), // Model
                                                new(flat[30], new(30, 31), new(12, 14)), // Model
                                            #endregion Model
                                        #endregion Model
                                    #endregion World3D
                                #endregion #ShadowRoot
                            #endregion Scene
                        #endregion SubViewport
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        ];

        actual = [.. cache.NodesList.Select(ToNodeRange)];

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        var flat14 = (Renderable)flat[14];

        flat14.Visible = false;

        cache.Build();

        flat = TreeFactory.Flatten(window).IgnoreEmpty();

        expected =
        [
            #region Window
            new(flat[0], new(0, 28)),
                #region Scene
                new(flat[1], new(1, 28)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 17)),
                        new(flat[3], new(3, 4)), // World3D

                        #region World2D
                        new(flat[4], new(4, 12)),
                            #region Sprite
                            new(flat[5], new(5, 12), new(0, 2, 14)),
                                #region Sprite
                                new(flat[6], new(6, 9), new(2, 4, 8)),
                                    new(flat[7], new(7, 8), new(4, 6)), // Sprite
                                    new(flat[8], new(8, 9), new(6, 8)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[9], new(9, 12), new(8, 10, 14)),
                                    new(flat[10], new(10, 11), new(10, 12)), // Sprite
                                    new(flat[11], new(11, 12), new(12, 14)), // Sprite
                                #endregion Sprite
                            #endregion Sprite
                        #endregion World2D

                        #region Canvas
                        new(flat[12], new(12, 17), new(0, 1, 13)),
                            #region Component
                            new(flat[13], new(13, 17), new(1, 2, 11, 13)),
                                #region Component
                                new(flat[14], new(14, 17), new(2, 3, 9, 11)),
                                    new(flat[15], new(15, 16), new(3, 4, 4, 6)), // Component
                                    new(flat[16], new(16, 17), new(6, 7, 7, 9)), // Component
                                #endregion Component
                            #endregion Component
                        #endregion Canvas

                        #region SubViewport
                        new(flat[17], new(17, 28)),
                            #region Scene
                            new(flat[18], new(18, 28)),
                                #region #ShadowRoot
                                new(flat[19], new(19, 28)),
                                    #region World3D
                                    new(flat[20], new(20, 28)),
                                        #region Model
                                        new(flat[21], new(21, 28), new(0, 2, 14)),
                                            #region Model
                                            new(flat[22], new(22, 25), new(2, 4, 8)),
                                                new(flat[23], new(23, 24), new(4, 6)), // Model
                                                new(flat[24], new(24, 25), new(6, 8)), // Model
                                            #endregion Model

                                            #region Model
                                            new(flat[25], new(25, 28), new(8, 10, 14)),
                                                new(flat[26], new(26, 27), new(10, 12)), // Model
                                                new(flat[27], new(27, 28), new(12, 14)), // Model
                                            #endregion Model
                                        #endregion Model
                                    #endregion World3D
                                #endregion #ShadowRoot
                            #endregion Scene
                        #endregion SubViewport
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        ];

        actual = [.. cache.NodesList.Select(ToNodeRange)];

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Rebuild_RebuildsAfterTreeChanges()
    {
        var window = Window.CreateMock();

        var cache = RenderTreeAccessor.GetSceneGraphCache(window.RenderTree);

        window.Scene = new()
        {
            World2D = new()
            {
                Name     = "$2D",
                Children =
                [
                    TreeFactory.Linear<Sprite, Command2D, SpriteCommand>(2, 2, 3, -1, "$2D.1"),
                ]
            }
        };

        var flat = TreeFactory.Flatten(window).IgnoreEmpty();;

        cache.InvalidatedSubTree(window);
        cache.Build();

        NodeRange[] expected =
        [
            #region Window
            new(flat[0], new(0, 11)),
                #region Scene
                new(flat[1], new(1, 11)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 11)),
                        #region World2D
                        new(flat[3], new(3, 11)),
                            #region Sprite
                            new(flat[4], new(4, 11), new(0, 3, 21)),
                                #region Sprite
                                new(flat[5], new(5, 8), new(3, 6, 12)),
                                    new(flat[6], new(6, 7), new(6, 9)), // Sprite
                                    new(flat[7], new(7, 8), new(9, 12)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[8], new(8, 11), new(12, 15, 21)),
                                    new(flat[9], new(9, 10), new(15, 18)), // Sprite
                                    new(flat[10], new(10, 11), new(18, 21)), // Sprite
                                #endregion Sprite
                            #endregion Sprite
                        #endregion World2D
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        ];

        var actual = cache.NodesList.Select(ToNodeRange).ToArray();

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        var component = TreeFactory.Linear<Component, Command2D, ComponentCommand>(2, 2, 2, -1, "$2D.1.1.1.1");

        flat[4].AppendChild(component);

        cache.Build();

        flat = TreeFactory.Flatten(window).IgnoreEmpty();;

        expected =
        [
            #region Window
            new(flat[0], new(0, 18)),
                #region Scene
                new(flat[1], new(1, 18)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 18)),
                        #region World2D
                        new(flat[3], new(3, 18)),
                            #region Sprite
                            new(flat[4], new(4, 18), new(0, 3, 35)),
                                #region Sprite
                                new(flat[5], new(5, 8), new(3, 6, 12)),
                                    new(flat[6], new(6, 7), new(6, 9)), // Sprite
                                    new(flat[7], new(7, 8), new(9, 12)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[8], new(8, 11), new(12, 15, 21)),
                                    new(flat[9], new(9, 10), new(15, 18)), // Sprite
                                    new(flat[10], new(10, 11), new(18, 21)), // Sprite
                                #endregion Sprite

                                #region Component
                                new(flat[11], new(11, 18), new(21, 23, 35)),
                                    #region Component
                                    new(flat[12], new(12, 15), new(23, 25, 29)),
                                        new(flat[13], new(13, 14), new(25, 27)), // Component
                                        new(flat[14], new(14, 15), new(27, 29)), // Component
                                    #endregion Component

                                    #region Component
                                    new(flat[15], new(15, 18), new(29, 31, 35)),
                                        new(flat[16], new(16, 17), new(31, 33)), // Component
                                        new(flat[17], new(17, 18), new(33, 35)), // Component
                                    #endregion Component
                                #endregion Component
                            #endregion Sprite
                        #endregion World2D
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        ];

        actual = [..cache.NodesList.Select(ToNodeRange)];

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        var flat15 = (Component)flat[15];
        var flat10 = (Sprite)flat[10];

        for (var i = 0; i < 4; i++)
        {
            RenderableAcessor<Command2D>.AddCommand(flat15, new SpriteCommand());
            RenderableAcessor<Command2D>.AddCommand(flat10, new SpriteCommand());
        }

        flat15.DirtState = DirtState.Commands;
        flat10.DirtState = DirtState.Commands;

        RenderableAcessor<Command2D>.SetCommandsSeparator(flat15, 3);
        RenderableAcessor<Command2D>.SetCommandsSeparator(flat10, 3);

        cache.InvalidatedSubTree(flat10);
        cache.InvalidatedSubTree(flat15);
        cache.Build();

        expected =
        [
            #region Window
            new(flat[0], new(0, 18)),
                #region Scene
                new(flat[1], new(1, 18)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 18)),
                        #region World2D
                        new(flat[3], new(3, 18)),
                            #region Sprite
                            new(flat[4], new(4, 18), new(0, 3, 39)),
                                #region Sprite
                                new(flat[5], new(5, 8), new(3, 6, 12)),
                                    new(flat[6], new(6, 7), new(6, 9)), // Sprite
                                    new(flat[7], new(7, 8), new(9, 12)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[8], new(8, 11), new(12, 15, 25)),
                                    new(flat[9], new(9, 10), new(15, 18)), // Sprite
                                    new(flat[10], new(10, 11), new(18, 21, 21, 25)), // Sprite
                                #endregion Sprite

                                #region Component
                                new(flat[11], new(11, 18), new(25, 27, 39)),
                                    #region Component
                                    new(flat[12], new(12, 15), new(27, 29, 33)),
                                        new(flat[13], new(13, 14), new(29, 31)), // Component
                                        new(flat[14], new(14, 15), new(31, 33)), // Component
                                    #endregion Component

                                    #region Component
                                    new(flat[15], new(15, 18), new(33, 35, 39)),
                                        new(flat[16], new(16, 17), new(35, 37)), // Component
                                        new(flat[17], new(17, 18), new(37, 39)), // Component
                                    #endregion Component
                                #endregion Component
                            #endregion Sprite
                        #endregion World2D
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        ];

        actual = [..cache.NodesList.Select(ToNodeRange)];

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DirtCommands_UpdatesCommandsOnDirtyNodes()
    {
        var window = Window.CreateMock();

        var cache = RenderTreeAccessor.GetSceneGraphCache(window.RenderTree);

        window.Scene = new()
        {
            World2D = new()
            {
                Name     = "$2D",
                Children =
                [
                    TreeFactory.Linear<Sprite, Command2D, SpriteCommand>(2, 2, 3, -1, "$2D.1"),
                ]
            }
        };

        var flat = TreeFactory.Flatten(window).IgnoreEmpty();;

        cache.InvalidatedSubTree(window);
        cache.Build();

        var expected = new NodeRange[11]
        {
            #region Window
            new(flat[0], new(0, 11)),
                #region Scene
                new(flat[1], new(1, 11)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 11)),
                        #region World2D
                        new(flat[3], new(3, 11)),
                            #region Sprite
                            new(flat[4], new(4, 11), new(0, 3, 21)),
                                #region Sprite
                                new(flat[5], new(5, 8), new(3, 6, 12)),
                                    new(flat[6], new(6, 7), new(6, 9)), // Sprite
                                    new(flat[7], new(7, 8), new(9, 12)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[8], new(8, 11), new(12, 15, 21)),
                                    new(flat[9], new(9, 10), new(15, 18)), // Sprite
                                    new(flat[10], new(10, 11), new(18, 21)), // Sprite
                                #endregion Sprite
                            #endregion Sprite
                        #endregion World2D
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        };

        var actual = cache.NodesList.Select(ToNodeRange).ToArray();

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        var renderable = (Renderable<Command2D>)flat[6];

        for (var i = 0; i < 3; i++)
        {
            RenderableAcessor<Command2D>.AddCommand(renderable, new SpriteCommand() { CommandFilter = CommandFilter.Color });
        }

        renderable.DirtState = DirtState.Commands;

        cache.InvalidatedSubTree(renderable);
        cache.Build();

        flat = TreeFactory.Flatten(window).IgnoreEmpty();;

        expected =
        [
            #region Window
            new(flat[0], new(0, 11)),
                #region Scene
                new(flat[1], new(1, 11)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 11)),
                        #region World2D
                        new(flat[3], new(3, 11)),
                            #region Sprite
                            new(flat[4], new(4, 11), new(0, 3, 24)),
                                #region Sprite
                                new(flat[5], new(5, 8), new(3, 6, 15)),
                                    new(flat[6], new(6, 7), new(6, 12)), // Sprite
                                    new(flat[7], new(7, 8), new(12, 15)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[8], new(8, 11), new(15, 18, 24)),
                                    new(flat[9], new(9, 10), new(18, 21)), // Sprite
                                    new(flat[10], new(10, 11), new(21, 24)), // Sprite
                                #endregion Sprite
                            #endregion Sprite
                        #endregion World2D
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        ];

        actual = [.. cache.NodesList.Select(ToNodeRange)];

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        var component = new Component();

        for (var i = 0; i < 3; i++)
        {
            RenderableAcessor<Command2D>.AddCommand(component, new SpriteCommand() { CommandFilter = CommandFilter.Color });
        }

        RenderableAcessor<Command2D>.SetCommandsSeparator(component, 3);

        renderable.AppendChild(component);

        cache.InvalidatedSubTree(renderable);
        cache.Build();

        flat = TreeFactory.Flatten(window).IgnoreEmpty();;

        expected =
        [
            #region Window
            new(flat[0], new(0, 12)),
                #region Scene
                new(flat[1], new(1, 12)),
                    #region #ShadowRoot
                    new(flat[2], new(2, 12)),
                        #region World2D
                        new(flat[3], new(3, 12)),
                            #region Sprite
                            new(flat[4], new(4, 12), new(0, 3, 27)),
                                #region Sprite
                                new(flat[5], new(5, 9), new(3, 6, 18)),
                                    #region Sprite
                                    new(flat[6], new(6, 8), new(6, 12, 15)),
                                        new(flat[7], new(7, 8), new(12, 15)), // Component
                                    #endregion Sprite

                                    new(flat[8], new(8, 9), new(15, 18)), // Sprite
                                #endregion Sprite

                                #region Sprite
                                new(flat[9], new(9, 12), new(18, 21, 27)),
                                    new(flat[10], new(10, 11), new(21, 24)), // Sprite
                                    new(flat[11], new(11, 12), new(24, 27)), // Sprite
                                #endregion Sprite
                            #endregion Sprite
                        #endregion World2D
                    #endregion #ShadowRoot
                #endregion Scene
            #endregion Window
        ];

        actual = [..cache.NodesList.Select(ToNodeRange)];

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InvalidatedSubTree_AccumulatesDirtTrees()
    {
        var window = Window.CreateMock();

        var cache = RenderTreeAccessor.GetSceneGraphCache(window.RenderTree);

        window.Scene = new()
        {
            World2D = new()
            {
                Name     = "$2D",
                Children =
                [
                    TreeFactory.Linear<Sprite, Command2D, SpriteCommand>(5, 1, 3, -1, "$2D.1"),
                ]
            }
        };

        var flat = TreeFactory.Flatten(window).IgnoreEmpty();;

        var flat2 = (Renderable)flat[2];
        var flat4 = (Renderable)flat[4];
        var flat7 = (Renderable)flat[7];

        flat2.DirtState = DirtState.Commands;
        flat7.DirtState = DirtState.Commands;
        flat4.DirtState = DirtState.Commands;

        cache.InvalidatedSubTree(flat4);
        cache.InvalidatedSubTree(flat7);
        cache.InvalidatedSubTree(flat2);

        var dirtTrees = SceneGraphCacheAccessor.GetDirtTrees(cache);

        Assert.Equal(DirtState.Commands | DirtState.Subtree, flat2.DirtState);
        Assert.Equal([flat[2]], dirtTrees);

        dirtTrees.Clear();

        var component = new SealedComponent("");

        flat7.AppendChild(component);

        var composedChild = (Renderable)component.ShadowRoot!.FirstChild!;

        component.DirtState     = DirtState.Commands;
        composedChild.DirtState = DirtState.Commands;

        cache.InvalidatedSubTree(component);
        cache.InvalidatedSubTree(composedChild);

        Assert.Equal(DirtState.Commands | DirtState.Subtree, flat2.DirtState);
        Assert.Equal([component], dirtTrees);

        dirtTrees.Clear();

        component.DirtState     = DirtState.Commands;
        composedChild.DirtState = DirtState.Commands;

        cache.InvalidatedSubTree(composedChild);
        cache.InvalidatedSubTree(component);

        Assert.Equal(DirtState.Commands | DirtState.Subtree, flat2.DirtState);
        Assert.Equal([component], dirtTrees);
    }
}
