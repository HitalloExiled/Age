using System.Runtime.CompilerServices;
using Age.Commands;
using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private record ComponentCommand : Command2D;
    private record SpriteCommand : Command2D;
    private record ModelCommand : Command3D;

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

        return new(node, subtree, commandRange.Color, commandRange.Encode);
    }

    [Fact]
    public void BuildOnce()
    {
        var cache = new SceneGraphCache();

        var window = new Window
        {
            Scene3D = new()
            {
                Name = "$3D",
            },
            Scene2D = new()
            {
                Name = "$2D",
                Children =
                [
                    TreeFactory<Sprite, Command2D, SpriteCommand>.Linear(2, 2, 2, null, CommandFilter.Color | CommandFilter.Encode, "$2D.1"),
                    new SubViewport("$2D.2")
                    {
                        Scene3D = new()
                        {
                            Name     = "$2D.2.#3D",
                            Children =
                            [
                                TreeFactory<Model, Command3D, MeshCommand>.Linear(2, 2, 2, null, CommandFilter.Color, "$2D.2.#3D.01"),
                            ],
                        }
                    },
                    TreeFactory<SealedComponent, Command2D, ComponentCommand>.Linear(1, 1, 3, 1, CommandFilter.Color | CommandFilter.Encode, "$2D.3"),
                    TreeFactory<Component, Command2D, ComponentCommand>.Linear(2, 2, 2, null, CommandFilter.Color | CommandFilter.Encode, "$2D.4"),
                ]
            },
        };

        window.Connect();

        cache.InvalidatedSubTree(window);
        cache.Build();

        var flat     = TreeFactory.Flatten(window);
        var expected = new NodeRange[44]
        {
            new(flat[0], new(0, 44)), // Window
                new(flat[1], new(1, 2)), // Scene3D

                new(flat[2], new(2, 44)), // Scene2D
                    new(flat[3], new(3, 10), new(0, 2, 14), new(0, 2, 14)), // Sprite
                        new(flat[4], new(4, 7), new(2, 4, 8, 8), new(2, 4, 8, 8)), // Sprite
                            new(flat[5], new(5, 6), new(4, 6), new(4, 6)), // Sprite
                            new(flat[6], new(6, 7), new(6, 8), new(6, 8)), // Sprite

                        new(flat[7], new(7, 10), new(8, 10, 14), new(8, 10, 14)), // Sprite
                            new(flat[8], new(8, 9), new(10, 12), new(10, 12)), // Sprite
                            new(flat[9], new(9, 10), new(12, 14), new(12, 14)), // Sprite

                    new(flat[10], new(10, 19)), // SubViewport
                        new(flat[11], new(11, 19)), // Scene3D
                            new(flat[12], new(12, 19), new(0, 2, 14), default), // Model
                                new(flat[13], new(13, 16), new(2, 4, 8), default), // Model
                                    new(flat[14], new(14, 15), new(4, 6), default), // Model
                                    new(flat[15], new(15, 16), new(6, 8), default), // Model

                                new(flat[16], new(16, 19), new(8, 10, 14), default), // Model
                                    new(flat[17], new(17, 18), new(10, 12), default), // Model
                                    new(flat[18], new(18, 19), new(12, 14), default), // Model

                    new(flat[19], new(19, 37), new(14, 15, 102, 104), new(14, 15, 66, 68)), // SealedComponent
                        new(flat[20], new(20, 24), new(15, 18, 36, 39), new(15, 18, 36, 39)), // Component
                            new(flat[21], new(21, 22), new(18, 21, 21, 24), new(18, 21, 21, 24)), // Component
                            new(flat[22], new(22, 23), new(24, 27, 27, 30), new(24, 27, 27, 30)), // Component
                            new(flat[23], new(23, 24), new(30, 33, 33, 36), new(30, 33, 33, 36)), // Component

                        new(flat[24], new(24, 27), new(39, 41, 53, 57), new(39)), // Component
                            new(flat[25], new(25, 26), new(41, 43, 43, 47), new(39)), // Component
                            new(flat[26], new(26, 27), new(47, 49, 49, 53), new(39)), // Component

                        new(flat[27], new(27, 37), new(57, 57, 102, 102), new(39, 39, 66, 66)), // Slot
                            new(flat[28], new(28, 37), new(57, 58, 100, 102), new(39, 40, 64, 66)), // SealedComponent
                                new(flat[29], new(29, 33), new(58, 61, 79, 82), new(40, 43, 61, 64)), // Component
                                    new(flat[30], new(30, 31), new(61, 64, 64, 67), new(43, 46, 46, 49)), // Component
                                    new(flat[31], new(31, 32), new(67, 70, 70, 73), new(49, 52, 52, 55)), // Component
                                    new(flat[32], new(32, 33), new(73, 76, 76, 79), new(55, 58, 58, 61)), // Component

                                new(flat[33], new(33, 36), new(82, 84, 96, 100), new(64)), // Component
                                    new(flat[34], new(34, 35), new(84, 86, 86, 90), new(64)), // Component
                                    new(flat[35], new(35, 36), new(90, 92, 92, 96), new(64)), // Component

                                new(flat[36], new(36, 37), new(100), new(64)), // Slot

                    new(flat[37], new(37, 44), new(104, 106, 118), new(68, 70, 82)), // Component
                        new(flat[38], new(38, 41), new(106, 108, 112), new(70, 72, 76)), // Component
                            new(flat[39], new(39, 40), new(108, 110), new(72, 74)), // Component
                            new(flat[40], new(40, 41), new(110, 112), new(74, 76)), // Component

                        new(flat[41], new(41, 44), new(112, 114, 118), new(76, 78, 82)), // Component
                            new(flat[42], new(42, 43), new(114, 116), new(78, 80)), // Component
                            new(flat[43], new(43, 44), new(116, 118), new(80, 82)), // Component
        };

        var actual = cache.Nodes.Select(ToNodeRange).ToArray();

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        var parent = (Renderable)flat[19].Parent!;

        flat[19].Detach();

        parent.DirtState = DirtState.Subtree;

        cache.InvalidatedSubTree(parent);
        cache.Build();

        flat = TreeFactory.Flatten(window);

        expected =
        [
            new(flat[0], new(0, 26)), // Window
                new(flat[1], new(1, 2)), // Scene3D

                new(flat[2], new(2, 26)), // Scene2D
                    new(flat[3], new(3, 10), new(0, 2, 14), new(0, 2, 14)), // Sprite
                        new(flat[4], new(4, 7), new(2, 4, 8, 8), new(2, 4, 8, 8)), // Sprite
                            new(flat[5], new(5, 6), new(4, 6), new(4, 6)), // Sprite
                            new(flat[6], new(6, 7), new(6, 8), new(6, 8)), // Sprite

                        new(flat[7], new(7, 10), new(8, 10, 14), new(8, 10, 14)), // Sprite
                            new(flat[8], new(8, 9), new(10, 12), new(10, 12)), // Sprite
                            new(flat[9], new(9, 10), new(12, 14), new(12, 14)), // Sprite

                    new(flat[10], new(10, 19)), // SubViewport
                        new(flat[11], new(11, 19)), // Scene3D
                            new(flat[12], new(12, 19), new(0, 2, 14)), // Model
                                new(flat[13], new(13, 16), new(2, 4, 8)), // Model
                                    new(flat[14], new(14, 15), new(4, 6)), // Model
                                    new(flat[15], new(15, 16), new(6, 8)), // Model

                                new(flat[16], new(16, 19), new(8, 10, 14)), // Model
                                    new(flat[17], new(17, 18), new(10, 12)), // Model
                                    new(flat[18], new(18, 19), new(12, 14)), // Model

                    new(flat[19], new(19, 26), new(14, 16, 28), new(14, 16, 28)), // Component
                        new(flat[20], new(20, 23), new(16, 18, 22), new(16, 18, 22)), // Component
                            new(flat[21], new(21, 22), new(18, 20), new(18, 20)), // Component
                            new(flat[22], new(22, 23), new(20, 22), new(20, 22)), // Component

                        new(flat[23], new(23, 26), new(22, 24, 28), new(22, 24, 28)), // Component
                            new(flat[24], new(24, 25), new(24, 26), new(24, 26)), // Component
                            new(flat[25], new(25, 26), new(26, 28), new(26, 28)), // Component
        ];

        actual = [.. cache.Nodes.Select(ToNodeRange)];

        // Uncomment if you is lost
        Emit(expected, actual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Rebuild()
    {
        var cache = new SceneGraphCache();

        var window = new Window
        {
            Name    = "$",
            Scene2D = new()
            {
                Name     = "$2D",
                Children =
                [
                    TreeFactory<Sprite, Command2D, SpriteCommand>.Linear(2, 2, 3, null, CommandFilter.Color, "$2D.1"),
                ]
            }
        };

        window.Connect();

        var flat = TreeFactory.Flatten(window);

        cache.InvalidatedSubTree(window);
        cache.Build();

        var expected = new NodeRange[9]
        {
            new(flat[0], new(0, 9)), // Window
                new(flat[1], new(1, 9)), // Scene2D
                    new(flat[2], new(2, 9), new(0, 3, 21)), // Sprite
                        new(flat[3], new(3, 6), new(3, 6, 12)), // Sprite
                            new(flat[4], new(4, 5), new(6, 9)), // Sprite
                            new(flat[5], new(5, 6), new(9, 12)), // Sprite

                        new(flat[6], new(6, 9), new(12, 15, 21)), // Sprite
                            new(flat[7], new(7, 8), new(15, 18)), // Sprite
                            new(flat[8], new(8, 9), new(18, 21)), // Sprite
        };

        var actual = cache.Nodes.Select(ToNodeRange).ToArray();

        var component = TreeFactory<Component, Command2D, ComponentCommand>.Linear(2, 2, 2, null, CommandFilter.Color | CommandFilter.Encode, "$2D.1.1.1.1");

        flat[4].AppendChild(component);

        cache.InvalidatedSubTree((Renderable)flat[4]);
        cache.Build();

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        flat = TreeFactory.Flatten(window);

        expected =
        [
            new(flat[0], new(0, 16)), // Window
                new(flat[1], new(1, 16)), // Scene2D
                    new(flat[2], new(2, 16), new(0, 3, 35), new(0, 0, 14)), // Sprite
                        new(flat[3], new(3, 13), new(3, 6, 26), new(0, 0, 14)), // Sprite
                            new(flat[4], new(4, 12), new(6, 9, 23), new(0, 0, 14)), // Sprite
                                new(flat[5], new(5, 12), new(9, 11, 23), new(0, 2, 14)), // Component
                                    new(flat[6], new(6, 9), new(11, 13, 17), new(2, 4, 8)), // Component
                                        new(flat[7], new(7, 8), new(13, 15), new(4, 6)), // Component
                                        new(flat[8], new(8, 9), new(15, 17), new(6, 8)), // Component

                                    new(flat[9], new(9, 12), new(17, 19, 23), new(8, 10, 14)), // Component
                                        new(flat[10], new(10, 11), new(19, 21), new(10, 12)), // Component
                                        new(flat[11], new(11, 12), new(21, 23), new(12, 14)), // Component

                            new(flat[12], new(12, 13), new(23, 26), new(14)), // Sprite

                        new(flat[13], new(13, 16), new(26, 29, 35), new(14)), // Sprite
                            new(flat[14], new(14, 15), new(29, 32), new(14)), // Sprite
                            new(flat[15], new(15, 16), new(32, 35), new(14)), // Sprite
        ];

        actual = [..cache.Nodes.Select(ToNodeRange)];

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DirtCommands()
    {
        var cache = new SceneGraphCache();

        var window = new Window
        {
            Name    = "$",
            Scene2D = new()
            {
                Name     = "$2D",
                Children =
                [
                    TreeFactory<Sprite, Command2D, SpriteCommand>.Linear(2, 2, 3, null, CommandFilter.Color, "$2D.1"),
                ]
            }
        };

        window.Connect();

        var flat = TreeFactory.Flatten(window);

        cache.InvalidatedSubTree(window);
        cache.Build();

        var expected = new NodeRange[9]
        {
            new(flat[0], new(0, 9)), // Window
                new(flat[1], new(1, 9)), // Scene2D
                    new(flat[2], new(2, 9), new(0, 3, 21)), // Sprite
                        new(flat[3], new(3, 6), new(3, 6, 12)), // Sprite
                            new(flat[4], new(4, 5), new(6, 9)), // Sprite
                            new(flat[5], new(5, 6), new(9, 12)), // Sprite
                        new(flat[6], new(6, 9), new(12, 15, 21)), // Sprite
                            new(flat[7], new(7, 8), new(15, 18)), // Sprite
                            new(flat[8], new(8, 9), new(18, 21)), // Sprite
        };

        var actual = cache.Nodes.Select(ToNodeRange).ToArray();

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        var renderable = (Renderable<Command2D>)flat[4];

        for (var i = 0; i < 3; i++)
        {
            RenderableAcessor<Command2D>.AddCommand(renderable, new SpriteCommand() { CommandFilter = CommandFilter.Color });
        }

        renderable.DirtState = DirtState.Commands;

        cache.InvalidatedSubTree(renderable);
        cache.Build();

        flat = TreeFactory.Flatten(window);

        expected =
        [
            new(flat[0], new(0, 9)), // Window
                new(flat[1], new(1, 9)), // Scene2D
                    new(flat[2], new(2, 9), new(0, 3, 24)), // Sprite
                        new(flat[3], new(3, 6), new(3, 6, 15)), // Sprite
                            new(flat[4], new(4, 5), new(6, 12)), // Sprite
                            new(flat[5], new(5, 6), new(12, 15)), // Sprite
                        new(flat[6], new(6, 9), new(15, 18, 24)), // Sprite
                            new(flat[7], new(7, 8), new(18, 21)), // Sprite
                            new(flat[8], new(8, 9), new(21, 24)), // Sprite
        ];

        actual = [.. cache.Nodes.Select(ToNodeRange)];

        // Uncomment if you is lost
        // Emit(expected, actual);

        Assert.Equal(expected, actual);

        var component = new Component();

        for (var i = 0; i < 3; i++)
        {
            RenderableAcessor<Command2D>.AddCommand(component, new SpriteCommand() { CommandFilter = CommandFilter.Color });
        }

        ElementAcessor.GetCommandsSeparator(component) = 3;

        renderable.AppendChild(component);

        cache.InvalidatedSubTree(renderable);
        cache.Build();

        flat = TreeFactory.Flatten(window);

        expected =
        [
            new(flat[0], new(0, 10)), // Window
                new(flat[1], new(1, 10)), // Scene2D
                    new(flat[2], new(2, 10), new(0, 3, 27)), // Sprite
                        new(flat[3], new(3, 7), new(3, 6, 18)), // Sprite
                            new(flat[4], new(4, 6), new(6, 12, 15)), // Sprite
                                new(flat[5], new(5, 6), new(12, 15)), // Component

                            new(flat[6], new(6, 7), new(15, 18)), // Sprite

                        new(flat[7], new(7, 10), new(18, 21, 27)), // Sprite
                            new(flat[8], new(8, 9), new(21, 24)), // Sprite
                            new(flat[9], new(9, 10), new(24, 27)), // Sprite
        ];

        actual = [..cache.Nodes.Select(ToNodeRange)];

        // Uncomment if you is lost
        Emit(expected, actual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InvalidatedSubTree()
    {
        var cache = new SceneGraphCache();

        var window = new Window
        {
            Name    = "$",
            Scene2D = new()
            {
                Name     = "$2D",
                Children =
                [
                    TreeFactory<Sprite, Command2D, SpriteCommand>.Linear(5, 1, 3, null, CommandFilter.Color, "$2D.1"),
                ]
            }
        };

        var flat = TreeFactory.Flatten(window);

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

        var component = new SealedComponent();

        flat7.AppendChild(component);

        var composedChild = (Renderable)component.ShadowTree!.FirstChild!;

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
