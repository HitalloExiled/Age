using Age.Elements;
using Age.Scene;
using BenchmarkDotNet.Attributes;

#pragma warning disable CA1001

namespace Age.Benchmarks;


public class TestTree : NodeTree
{
    protected override void Disposed(bool disposing) => throw new NotImplementedException();
}

public class TestElement : Element
{
    public override string NodeName { get; } = nameof(TestElement);
}

public class HostElement : Element
{
    public override string NodeName { get; } = nameof(HostElement);

    public HostElement()
    {
        this.AttachShadowTree();

        this.ShadowTree.Children =
        [
            new TestElement
            {
                Name     = "#.0",
                Children =
                [],
            },
            new TestElement
            {
                Name     = "#.1",
                Children =
                [
                    new TestElement { Name = "#.1.1" },
                    new TestElement
                    {
                        Name     = "#.1.2",
                        Children =
                        [
                            new Slot
                            {
                                Name     = "#.1.2.(0)",
                                Children =
                                [
                                    new TestElement { Name = "ignored" },
                                ],
                            }
                        ],
                    },
                    new TestElement { Name = "#.1.3" },
                ],
            },
            new TestElement
            {
                Name     = "#.2",
                Children =
                [
                    new TestElement
                    {
                        Name     = "#.2.1",
                        Children =
                        [
                            new Slot
                            {
                                Name     = "#.2.1.(0)",
                                Children =
                                [
                                    new TestElement { Name = "#.2.1.(1).1" },
                                ]
                            },
                        ],
                    },
                    new TestElement { Name = "#.2.2" },
                ],
            },
            new TestElement
            {
                Name     = "#.3",
                Children =
                [
                    new TestElement { Name = "#.3.1" },
                ],
            },
        ];
    }
}


[ShortRunJob]
[MemoryDiagnoser]
public class NodeShadowTreeTraversalBenchmarks
{
    private static IEnumerable<Node> Traverse(Node node)
    {
        foreach (var child in node)
        {
            yield return child;

            foreach (var granChild in Traverse(child))
            {
                yield return granChild;
            }
        }
    }

    private readonly TestTree tree = new();

    public NodeShadowTreeTraversalBenchmarks()
    {
        var host = new HostElement
        {
            Name     = "$",
            Children =
            [
                new TestElement
                {
                    Name     = "$.[#.1.2.(0)]",
                    Slot     = "#.1.2.(0)",
                    Children =
                    [
                        new TestElement { Name = "$.[#.1.2.(0)].1" },
                    ]
                },
                new TestElement
                {
                    Name     = "$.0",
                    Children =
                    [],
                },
                new TestElement
                {
                    Name     = "$.1",
                    Children =
                    [
                        new TestElement { Name = "$.1.1" },
                        new TestElement { Name = "$.1.2" },
                        new TestElement { Name = "$.1.3" },
                    ],
                },
                new TestElement
                {
                    Name     = "$.2",
                    Children =
                    [
                        new TestElement { Name = "$.2.1" },
                        new TestElement { Name = "$.2.2" },
                    ],
                },
                new TestElement
                {
                    Name     = "$.3",
                    Children =
                    [
                        new TestElement { Name = "$.3.1" },
                    ],
                },
            ]
        };

        var tree = new TestTree();

        tree.Root.AppendChild(host);
    }

    public static IEnumerable<Node> TraverseRecursive(Node root)
    {
        if (root is Element rootElement && rootElement.ShadowTree != null)
        {
            foreach (var item in TraverseRecursive(rootElement.ShadowTree))
            {
                yield return item;
            }
        }

        foreach (var node in root)
        {
            if (node is Element element)
            {
                if (element.AssignedSlot != null)
                {
                    continue;
                }

                if (element is Slot slot && slot.Nodes.Count > 0)
                {
                    yield return slot;

                    foreach (var slotted in slot.Nodes)
                    {
                        yield return slotted;

                        foreach (var slottedChild in TraverseRecursive(slotted))
                        {
                            yield return slottedChild;
                        }
                    }

                    continue;
                }
            }

            yield return node;

            foreach (var item in TraverseRecursive(node))
            {
                yield return item;
            }
        }
    }

    // Ignore this
    public static IEnumerable<Node> TraverseNonRecursive(Node root)
    {
        var stack = new Stack<(Node Node, bool IsSlotted)>();

        stack.Push((root, false));

        while (stack.Count > 0)
        {
            var (current, isSlotted) = stack.Pop();

            if (current is Element element)
            {
                if (!isSlotted && element.AssignedSlot != null)
                {
                    continue;
                }

                if (element is Slot slot && slot.Nodes.Count > 0)
                {
                    yield return slot;

                    for (var i = slot.Nodes.Count - 1; i >= 0; i--)
                    {
                        stack.Push((slot.Nodes[i], true));
                    }

                    continue;
                }

                if (element.ShadowTree != null)
                {
                    var node = element.LastChild;

                    while (node != null)
                    {
                        stack.Push((node, false));

                        node = node.PreviousSibling;
                    }

                    stack.Push((element.ShadowTree, false));

                    continue;
                }
            }

            if (current != root && current is not ShadowTree)
            {
                yield return current;
            }

            var child = current.LastChild;

            while (child != null)
            {
                stack.Push((child, false));

                child = child.PreviousSibling;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NodeTraversalRecursive()
    {
        var count = 0;

        foreach (var node in TraverseRecursive(this.tree.Root))
        {
            count++;
        }

        return count;
    }

    public int NodeTraversalNonRecursive()
    {
        var count = 0;

        foreach (var node in TraverseNonRecursive(this.tree.Root))
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int NodeTraversalIterator()
    {
        var count = 0;

        var enumerator = new Node.TraverseShadowTreeEnumerator(this.tree.Root);

        while (enumerator.MoveNext())
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int NodeTraversalIteratorInForeach()
    {
        var count = 0;

        foreach (var node in new Node.TraverseShadowTreeEnumerator(this.tree.Root))
        {
            count++;
        }

        return count;
    }
}
