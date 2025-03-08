using Age.Elements;
using Age.Scene;

namespace Age.Tests.Age.Scene;

#pragma warning disable CA1001

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

    public Element[] ShadowNodes { get; }

    public HostElement(string name)
    {
        this.AttachShadowTree();

        this.Name = name;

        TestElement       child1;
        TestElement       child21;
        TestElement       child22;
        Slot              child22Slot;
        TestElement       child23;
        TestElement       child2;
        TestElement       child31;
        Slot              child31Slot;
        TestElement       child31Slot1;
        TestElement       child32;
        TestElement       child3;
        NestedHostElement child4;
        TestElement       child5;
        TestElement       child51;

        this.ShadowTree.Children =
        [
            child1 = new TestElement { Name = $"{name}.#.1" },
            child2 = new TestElement
            {
                Name     = $"{name}.#.2",
                Children =
                [
                    child21 = new TestElement { Name = $"{name}.#.2.1" },
                    child22 = new TestElement
                    {
                        Name     = $"{name}.#.2.2",
                        Children =
                        [
                            child22Slot = new Slot
                            {
                                Name     = $"{name}.#.2.2.(1)",
                                Children =
                                [
                                    new TestElement { Name = "ignored" },
                                ],
                            }
                        ],
                    },
                    child23 = new TestElement { Name = $"{name}.#.2.3" },
                ],
            },
            child3 = new TestElement
            {
                Name     = $"{name}.#.3",
                Children =
                [
                    child31 = new TestElement
                    {
                        Name     = $"{name}.#.3.1",
                        Children =
                        [
                            child31Slot = new Slot
                            {
                                Name     = $"{name}.#.3.1.(1)",
                                Children =
                                [
                                    child31Slot1 = new TestElement { Name = $"{name}.#.3.1.(1).1" },
                                ]
                            },
                        ],
                    },
                    child32 = new TestElement { Name = $"{name}.#.3.2" },
                ],
            },
            child4 = new NestedHostElement($"{name}.#.4"),
            child5 = new TestElement
            {
                Name     = $"{name}.#.5",
                Children =
                [
                    child51 = new TestElement { Name = $"{name}.#.5.1" },
                ],
            },
        ];

        this.ShadowNodes =
        [
            child1,
            child2,
            child21,
            child22,
            child22Slot,
            child23,
            child3,
            child31,
            child31Slot,
            child31Slot1,
            child32,
            child4,
            child5,
            child51,
        ];
    }
}

public class NestedHostElement : Element
{
    public override string NodeName { get; } = nameof(NestedHostElement);

    public Element[] ShadowNodes { get; }

    public NestedHostElement(string name)
    {
        this.AttachShadowTree();

        TestElement nestedChild0;
        TestElement nestedChild1;
        TestElement nestedChild11;
        TestElement nestedChild12;
        TestElement nestedChild13;

        this.Name = name;

        this.ShadowTree.Children =
        [
            nestedChild0 = new TestElement
            {
                Name     = $"{name}.#.1",
                Children =
                [],
            },
            nestedChild1 = new TestElement
            {
                Name     = $"{name}.#.2",
                Children =
                [
                    nestedChild11 = new TestElement { Name = $"{name}.#.2.1" },
                    nestedChild12 = new TestElement { Name = $"{name}.#.2.2", },
                    nestedChild13 = new TestElement { Name = $"{name}.#.2.3" },
                ],
            },
        ];

        this.ShadowNodes =
        [
            nestedChild0,
            nestedChild1,
            nestedChild11,
            nestedChild12,
            nestedChild13,
        ];
    }
}

public class ShadowTreeTest
{
    private readonly TestTree tree = new();

    private readonly TestElement child1Slot1;
    private readonly TestElement child1Slot11;
    private readonly TestElement child2Slot1;
    private readonly TestElement child1;
    private readonly TestElement child11;
    private readonly TestElement child12;
    private readonly TestElement child13;
    private readonly TestElement child2;
    private readonly TestElement child21;
    private readonly TestElement child22;
    private readonly TestElement child3;
    private readonly TestElement child31;
    private readonly TestElement child4;
    private readonly HostElement host;

    public ShadowTreeTest()
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
                this.child1 = new TestElement { Name = "$.1", },
                this.child2 = new TestElement
                {
                    Name     = "$.2",
                    Children =
                    [
                        this.child11 = new TestElement { Name = "$.2.1" },
                        this.child12 = new TestElement { Name = "$.2.2" },
                        this.child13 = new TestElement { Name = "$.2.3" },
                    ],
                },
                this.child3 = new TestElement
                {
                    Name     = "$.3",
                    Children =
                    [
                        this.child21 = new TestElement { Name = "$.3.1" },
                        this.child22 = new TestElement { Name = "$.3.2" },
                    ],
                },
                this.child4 = new TestElement
                {
                    Name     = "$.4",
                    Children =
                    [
                        this.child31 = new TestElement { Name = "$.4.1" },
                    ],
                },
            ]
        };

        this.tree.Root.AppendChild(this.host);
    }

    [Fact]
    public void TraverseShadowTree()
    {
        var nestedHost = (NestedHostElement)this.host.ShadowNodes[11];

        Node[] shadowNodes =
        [
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
            this.child1,
            this.child2,
            this.child11,
            this.child12,
            this.child13,
            this.child3,
            this.child21,
            this.child22,
            this.child4,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Node.TraverseShadowTreeEnumerator(this.host);

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
            this.child1,
            this.child2,
            this.child11,
            this.child12,
            this.child13,
            this.child3,
            this.child21,
            this.child22,
            this.child4,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Node.TraverseShadowTreeEnumeratorV2(this.host);

        while (enumerator.MoveNext())
        {
            actual.Add(enumerator.Current.Name!);
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SkipToNextSibling()
    {
        Node[] shadowNodes =
        [
            ..this.host.ShadowNodes[0..4],
            ..this.host.ShadowNodes[5..8],
            ..this.host.ShadowNodes[10..],
        ];

        Node[] lightNodes  =
        [
            this.child1,
            this.child2,
            this.child11,
            this.child12,
            this.child13,
            this.child4,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        var enumerator = new Node.TraverseShadowTreeEnumerator(this.host);

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == this.child3 || enumerator.Current is Slot)
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
