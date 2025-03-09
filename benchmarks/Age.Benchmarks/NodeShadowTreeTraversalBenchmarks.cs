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
public class NodeShadowTreeTraversalBenchmarks
{
    private readonly TestTree tree = new();

    [Params(1, 3, 5)]
    public int Depth;

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
        var current = this.tree.Root;

        var depth = this.Depth;

        AddChilds(current, ref depth);
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

    [Benchmark]
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
    public int NodeTraversalIteratorV2()
    {
        var count = 0;

        var enumerator = new Node.TraverseShadowTreeEnumeratorV2(this.tree.Root);

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

    [Benchmark]
    public int NodeTraversalIteratorInForeachV2()
    {
        var count = 0;

        foreach (var node in new Node.TraverseShadowTreeEnumeratorV2(this.tree.Root))
        {
            count++;
        }

        return count;
    }
}
