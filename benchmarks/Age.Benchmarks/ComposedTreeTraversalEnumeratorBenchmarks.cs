using Age.Elements;
using Age.Scene;
using BenchmarkDotNet.Attributes;

#pragma warning disable CA1001

namespace Age.Benchmarks;

public class TestTree : NodeTree
{
    protected override void OnDisposed(bool disposing) => throw new NotImplementedException();
}

public class TestElement : Element
{
    public override string NodeName => nameof(TestElement);
}

public class RootElement : Element
{
    public override string NodeName => nameof(RootElement);
}

public class HostElement : Element
{
    public override string NodeName => nameof(HostElement);

    public HostElement()
    {
        this.AttachShadowTree();

        this.ShadowTree.Children =
        [
            new TestElement(),
            new TestElement
            {
                Children =
                [
                    new TestElement(),
                    new TestElement
                    {
                        Children =
                        [
                            new Slot
                            {
                                Name     = "slot",
                                Children =
                                [
                                    new TestElement(),
                                ],
                            }
                        ],
                    },
                    new TestElement(),
                ],
            },
            new TestElement
            {
                Children =
                [
                    new TestElement
                    {
                        Children =
                        [
                            new Slot
                            {
                                Name     = "unused",
                                Children =
                                [
                                    new TestElement(),
                                ]
                            },
                        ],
                    },
                    new TestElement(),
                ],
            },
            new TestElement
            {
                Children =
                [
                    new TestElement(),
                ],
            },
        ];
    }
}

[ShortRunJob]
[MemoryDiagnoser]
public class ComposedTreeTraversalEnumeratorBenchmarks
{
    private readonly RootElement root;
    private readonly TestTree    tree;

    [Params(1, 3, 5)]
    public int Depth;

    public ComposedTreeTraversalEnumeratorBenchmarks()
    {
        this.tree = new();
        this.root = new();

        this.tree.Root.AppendChild(this.root);
    }

    public static IEnumerable<Layoutable> TraverseRecursive(Node root)
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
            if (node is Layoutable layoutable)
            {
                if (layoutable.AssignedSlot != null)
                {
                    continue;
                }

                if (layoutable is Slot slot && slot.Nodes.Count > 0)
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

                yield return layoutable;
            }

            foreach (var item in TraverseRecursive(node))
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<Layoutable> TraverseNonRecursive(Node root)
    {
        var stack = new Stack<(Node Node, bool IsSlotted)>();

        stack.Push((root, false));

        while (stack.Count > 0)
        {
            var (current, isSlotted) = stack.Pop();

            if (current is Layoutable layoutable)
            {
                if (!isSlotted && layoutable.AssignedSlot != null)
                {
                    continue;
                }

                if (layoutable is Slot slot && slot.Nodes.Count > 0)
                {
                    yield return slot;

                    for (var i = slot.Nodes.Count - 1; i >= 0; i--)
                    {
                        stack.Push((slot.Nodes[i], true));
                    }

                    continue;
                }

                if (layoutable is Element element && element.ShadowTree != null)
                {
                    var node = layoutable.LastChild;

                    while (node != null)
                    {
                        stack.Push((node, false));

                        node = node.PreviousSibling;
                    }

                    stack.Push((element.ShadowTree, false));

                    continue;
                }

                yield return layoutable;
            }

            var child = current.LastChild;

            while (child != null)
            {
                stack.Push((child, false));

                child = child.PreviousSibling;
            }
        }
    }

    private static void AddChilds(Node parent, ref int parentDepth)
    {
        if (parentDepth > 0)
        {
            parentDepth--;

            for (var i = 0; i < 3; i++)
            {
                var child = new HostElement();

                parent.AppendChild(child);

                var depth = parentDepth;

                AddChilds(child, ref depth);
            }

            var slotted = new HostElement { Slot = "slot" };
            parent.AppendChild(slotted);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        var current = this.root;
        var depth   = this.Depth;

        AddChilds(current, ref depth);
    }

    [Benchmark(Baseline = true)]
    public int NodeTraversalRecursive()
    {
        var count = 0;

        foreach (var node in TraverseRecursive(this.root))
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int NodeTraversalNonRecursive()
    {
        var count = 0;

        foreach (var node in TraverseNonRecursive(this.root))
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int ComposedTreeTraversalEnumerator()
    {
        var count = 0;

        var enumerator = new ComposedTreeTraversalEnumerator(this.root);

        while (enumerator.MoveNext())
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int ComposedTreeTraversalEnumeratorInForeach()
    {
        var count = 0;

        foreach (var node in new ComposedTreeTraversalEnumerator(this.root))
        {
            count++;
        }

        return count;
    }
}
