using Age.Elements;
using Age.Scene;

namespace Age.Tests.Age.Scene;

#pragma warning disable CA1001

public partial class ComposedTreeTraversalEnumeratorTest
{
    private readonly TestTree tree = new();

    private readonly TestElement root = new();
    private readonly HostElement host;
    private readonly Node[]      lightNodes;

    public ComposedTreeTraversalEnumeratorTest()
    {
        TestElement child1Slot1;
        TestElement child1Slot11;
        TestElement child2Slot1;
        TestElement child3;
        TestElement child41;
        TestElement child42;
        TestElement child43;
        TestElement child4;
        TestElement child51;
        TestElement child52;
        TestElement child5;
        TestElement child61;
        TestElement child6;

        this.host = new HostElement("$")
        {
            Children =
            [
                child1Slot1 = new TestElement
                {
                    Name     = "$.1[#.2.2.(1)]",
                    Slot     = "$.#.2.2.(1)",
                    Children =
                    [
                        child1Slot11 = new TestElement { Name = "$.1[#.2.2.(1)].1" },
                    ]
                },
                child2Slot1 = new TestElement
                {
                    Name     = "$.2[#.2.2.(1)]",
                    Slot     = "$.#.2.2.(1)",
                },
                child3 = new TestElement { Name = "$.3", },
                child4 = new TestElement
                {
                    Name     = "$.4",
                    Children =
                    [
                        child41 = new TestElement { Name = "$.4.1" },
                        child42 = new TestElement { Name = "$.4.2" },
                        child43 = new TestElement { Name = "$.4.3" },
                    ],
                },
                child5 = new TestElement
                {
                    Name     = "$.5",
                    Children =
                    [
                        child51 = new TestElement { Name = "$.5.1" },
                        child52 = new TestElement { Name = "$.5.2" },
                    ],
                },
                child6 = new TestElement
                {
                    Name     = "$.6",
                    Children =
                    [
                        child61 = new TestElement { Name = "$.6.1" },
                    ],
                },
            ]
        };

        this.lightNodes =
        [
            child1Slot1,
            child1Slot11,
            child2Slot1,
            child3,
            child4,
            child41,
            child42,
            child43,
            child5,
            child51,
            child52,
            child6,
            child61,
        ];

        this.root.AppendChild(this.host);
        this.tree.Root.AppendChild(this.root);
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
    public void TraverseComposedTree()
    {
        var nestedHost = (NestedHostElement)this.host.ShadowNodes[11];

        Node[] nodes =
        [
            this.host,
            ..this.host.ShadowNodes[0..5],
            ..this.lightNodes[..3],
            ..this.host.ShadowNodes[5..12],
            ..nestedHost.ShadowNodes,
            ..this.host.ShadowNodes[12..],
            ..this.lightNodes[3..]
        ];

        var expected = nodes
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Layoutable.ComposedTreeTraversalEnumerator(this.root);

        while (enumerator.MoveNext())
        {
            actual.Add(enumerator.Current.Name!);
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SkipToNextSibling()
    {
        var nestedHost = this.host.ShadowNodes[11];
        var child5     = this.lightNodes[8];

        Node[] nodes =
        [
            ..this.host.ShadowNodes[0..4],
            ..this.host.ShadowNodes[5..8],
            this.host.ShadowNodes[10],
            ..this.host.ShadowNodes[12..],
            ..this.lightNodes[3..8],
            ..this.lightNodes[11..],
        ];

        var expected = nodes
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Layoutable.ComposedTreeTraversalEnumerator(this.host);

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == child5 || enumerator.Current == nestedHost || enumerator.Current is Slot)
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
}
