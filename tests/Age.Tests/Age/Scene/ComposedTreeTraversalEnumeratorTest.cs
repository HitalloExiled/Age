using Age.Elements;
using Age.Scene;

namespace Age.Tests.Age.Scene;

#pragma warning disable CA1001

public partial class ComposedTreeTraversalEnumeratorTest
{
    private readonly TestTree tree = new();

    private readonly TestElement child1Slot1;
    private readonly TestElement child1Slot11;
    private readonly TestElement child2Slot1;
    private readonly TestElement child3;
    private readonly TestElement child11;
    private readonly TestElement child12;
    private readonly TestElement child13;
    private readonly TestElement child4;
    private readonly TestElement child21;
    private readonly TestElement child22;
    private readonly TestElement child5;
    private readonly TestElement child31;
    private readonly TestElement child6;
    private readonly HostElement host;

    public ComposedTreeTraversalEnumeratorTest()
    {
        this.host = new HostElement("$")
        {
            Children =
            [
                this.child1Slot1 = new TestElement
                {
                    Name     = "$.1[#.2.2.(1)]",
                    Slot     = "$.#.2.2.(1)",
                    Children =
                    [
                        this.child1Slot11 = new TestElement { Name = "$.1[#.2.2.(1)].1" },
                    ]
                },
                this.child2Slot1 = new TestElement
                {
                    Name     = "$.2[#.2.2.(1)]",
                    Slot     = "$.#.2.2.(1)",
                },
                this.child3 = new TestElement { Name = "$.3", },
                this.child4 = new TestElement
                {
                    Name     = "$.4",
                    Children =
                    [
                        this.child11 = new TestElement { Name = "$.4.1" },
                        this.child12 = new TestElement { Name = "$.4.2" },
                        this.child13 = new TestElement { Name = "$.4.3" },
                    ],
                },
                this.child5 = new TestElement
                {
                    Name     = "$.5",
                    Children =
                    [
                        this.child21 = new TestElement { Name = "$.5.1" },
                        this.child22 = new TestElement { Name = "$.5.2" },
                    ],
                },
                this.child6 = new TestElement
                {
                    Name     = "$.6",
                    Children =
                    [
                        this.child31 = new TestElement { Name = "$.6.1" },
                    ],
                },
            ]
        };

        this.tree.Root.AppendChild(this.host);
    }

    private static void AddChilds(Node parent, ref int parentDepth)
    {
        if (parentDepth > 0)
        {
            parentDepth--;

            for (var i = 0; i < 3; i++)
            {
                var child   = new HostElement($"{parent.Name}.{i + 1}");

                parent.AppendChild(child);

                var depth = parentDepth;

                AddChilds(child, ref depth);
            }

            var slotted = new HostElement($"{parent.Name}.4") { Slot = $"{parent.Name}.#.2.2.(1)" };

            parent.AppendChild(slotted);
        }
    }

    [Fact]
    public void TraverseShadowTree()
    {
        var nestedHost = (NestedHostElement)this.host.ShadowNodes[11];

        Node[] shadowNodes =
        [
            this.host,
            ..this.host.ShadowNodes[0..5],
            this.child1Slot1,
            this.child1Slot11,
            this.child2Slot1,
            ..this.host.ShadowNodes[5..12],
            ..nestedHost.ShadowNodes,
            ..this.host.ShadowNodes[12..]
        ];

        Node[] lightNodes  =
        [
            this.child3,
            this.child4,
            this.child11,
            this.child12,
            this.child13,
            this.child5,
            this.child21,
            this.child22,
            this.child6,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Node.ComposedTreeTraversalEnumerator(this.tree.Root);

        while (enumerator.MoveNext())
        {
            actual.Add(enumerator.Current.Name!);
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TraverseShadowTreeV2()
    {
        var nestedHost = (NestedHostElement)this.host.ShadowNodes[11];

        Node[] shadowNodes =
        [
            this.host,
            ..this.host.ShadowNodes[0..5],
            this.child1Slot1,
            this.child1Slot11,
            this.child2Slot1,
            ..this.host.ShadowNodes[5..12],
            ..nestedHost.ShadowNodes,
            ..this.host.ShadowNodes[12..]
        ];

        Node[] lightNodes  =
        [
            this.child3,
            this.child4,
            this.child11,
            this.child12,
            this.child13,
            this.child5,
            this.child21,
            this.child22,
            this.child6,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Node.ComposedTreeTraversalEnumeratorV2(this.tree.Root);

        while (enumerator.MoveNext())
        {
            actual.Add(enumerator.Current.Name!);
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SkipToNextSibling()
    {
        var nestedHost = (NestedHostElement)this.host.ShadowNodes[11];

        Node[] shadowNodes =
        [
            ..this.host.ShadowNodes[0..4],
            ..this.host.ShadowNodes[5..8],
            this.host.ShadowNodes[10],
            ..this.host.ShadowNodes[12..],
        ];

        Node[] lightNodes  =
        [
            this.child3,
            this.child4,
            this.child11,
            this.child12,
            this.child13,
            this.child6,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Node.ComposedTreeTraversalEnumerator(this.host);

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == this.child5 || enumerator.Current == nestedHost || enumerator.Current is Slot)
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                actual.Add(enumerator.Current.Name!);
            }
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SkipToNextSiblingV2()
    {
        var nestedHost = (NestedHostElement)this.host.ShadowNodes[11];

        Node[] shadowNodes =
        [
            ..this.host.ShadowNodes[0..4],
            ..this.host.ShadowNodes[5..8],
            this.host.ShadowNodes[10],
            ..this.host.ShadowNodes[12..],
        ];

        Node[] lightNodes  =
        [
            this.child3,
            this.child4,
            this.child11,
            this.child12,
            this.child13,
            this.child6,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Node.ComposedTreeTraversalEnumeratorV2(this.host);

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == this.child5 || enumerator.Current == nestedHost || enumerator.Current is Slot)
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                actual.Add(enumerator.Current.Name!);
            }
        }

        Assert.Equal(expected, actual);
    }

    [Fact(Skip = "Ignored for now")]
    public void CompareV1toV2()
    {
        var tree1 = new TestTree();
        var tree2 = new TestTree();

        tree1.Root.Name = "$";
        tree2.Root.Name = "$";

        var depth1 = 10;
        var depth2 = 10;

        AddChilds(tree1.Root, ref depth1);
        AddChilds(tree2.Root, ref depth2);

        var list1 = new List<string>(2450493);
        var list2 = new List<string>(2450493);

        var enumerator1 = new Node.ComposedTreeTraversalEnumerator(tree1.Root);
        var enumerator2 = new Node.ComposedTreeTraversalEnumeratorV2(tree2.Root);

        while (enumerator1.MoveNext())
        {
            list1.Add(enumerator1.Current.Name!);
        }

        while (enumerator2.MoveNext())
        {
            list2.Add(enumerator2.Current.Name!);
        }

        Assert.Equal(list1.Count, list2.Count);
        Assert.Equal(list1, list2);
    }
}
