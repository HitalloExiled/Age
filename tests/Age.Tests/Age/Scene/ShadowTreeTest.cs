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

    public HostElement()
    {
        this.AttachShadowTree();

        TestElement child0;
        TestElement child11;
        TestElement child12;
        Slot        child12Slot;
        TestElement child13;
        TestElement child1;
        TestElement child21;
        Slot        child21Slot;
        TestElement child21Slot1;
        TestElement child22;
        TestElement child2;
        TestElement child31;
        TestElement child3;

        this.ShadowTree.Children =
        [
            child0 = new TestElement
            {
                Name     = "#.0",
                Children =
                [],
            },
            child1 = new TestElement
            {
                Name     = "#.1",
                Children =
                [
                    child11 = new TestElement { Name = "#.1.1" },
                    child12 = new TestElement
                    {
                        Name     = "#.1.2",
                        Children =
                        [
                            child12Slot = new Slot
                            {
                                Name     = "#.1.2.(0)",
                                Children =
                                [
                                    new TestElement { Name = "ignored" },
                                ],
                            }
                        ],
                    },
                    child13 = new TestElement { Name = "#.1.3" },
                ],
            },
            child2 = new TestElement
            {
                Name     = "#.2",
                Children =
                [
                    child21 = new TestElement
                    {
                        Name     = "#.2.1",
                        Children =
                        [
                            child21Slot = new Slot
                            {
                                Name     = "#.2.1.(0)",
                                Children =
                                [
                                    child21Slot1 = new TestElement { Name = "#.2.1.(1).1" },
                                ]
                            },
                        ],
                    },
                    child22 = new TestElement { Name = "#.2.2" },
                ],
            },
            child3 = new TestElement
            {
                Name     = "#.3",
                Children =
                [
                    child31 = new TestElement { Name = "#.3.1" },
                ],
            },
        ];

        this.ShadowNodes =
        [
            child0,
            child1,
            child11,
            child12,
            child12Slot,
            child13,
            child2,
            child21,
            child21Slot,
            child21Slot1,
            child22,
            child3,
            child31,
        ];
    }
}

public class ShadowTreeTest
{
    private readonly TestTree tree = new();

    private readonly TestElement slotted;
    private readonly TestElement slotted1;
    private readonly TestElement child0;
    private readonly TestElement child11;
    private readonly TestElement child12;
    private readonly TestElement child13;
    private readonly TestElement child1;
    private readonly TestElement child21;
    private readonly TestElement child22;
    private readonly TestElement child2;
    private readonly TestElement child31;
    private readonly TestElement child3;
    private readonly HostElement host;

    public ShadowTreeTest()
    {
        this.host = new HostElement
        {
            Name     = "$",
            Children =
            [
                this.slotted = new TestElement
                {
                    Name     = "$.[#.1.2.(0)]",
                    Slot     = "#.1.2.(0)",
                    Children =
                    [
                        this.slotted1 = new TestElement { Name = "$.[#.1.2.(0)].1" },
                    ]
                },
                this.child0 = new TestElement
                {
                    Name     = "$.0",
                    Children =
                    [],
                },
                this.child1 = new TestElement
                {
                    Name     = "$.1",
                    Children =
                    [
                        this.child11 = new TestElement { Name = "$.1.1" },
                        this.child12 = new TestElement { Name = "$.1.2" },
                        this.child13 = new TestElement { Name = "$.1.3" },
                    ],
                },
                this.child2 = new TestElement
                {
                    Name     = "$.2",
                    Children =
                    [
                        this.child21 = new TestElement { Name = "$.2.1" },
                        this.child22 = new TestElement { Name = "$.2.2" },
                    ],
                },
                this.child3 = new TestElement
                {
                    Name     = "$.3",
                    Children =
                    [
                        this.child31 = new TestElement { Name = "$.3.1" },
                    ],
                },
            ]
        };

        this.tree.Root.AppendChild(this.host);
    }

    [Fact]
    public void TraverseShadowTree()
    {
        Node[] shadowNodes =
        [
            ..this.host.ShadowNodes[0..5],
            this.slotted,
            this.slotted1,
            ..this.host.ShadowNodes[5..]
        ];

        Node[] lightNodes  =
        [
            this.child0,
            this.child1,
            this.child11,
            this.child12,
            this.child13,
            this.child2,
            this.child21,
            this.child22,
            this.child3,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        // foreach (var node in Node.TraverseShadowTreeEnumerator.Traverse(host))
        // {
        //     actual.Add(node.Name!);
        // }

        // Assert.Equal(expected, actual);

        // actual.Clear();

        // foreach (var node in Node.TraverseShadowTreeEnumerator.TraverseNonRecursive(host))
        // {
        //     actual.Add(node.Name!);
        // }

        var enumerator = new Node.TraverseShadowTreeEnumerator(this.host);

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
            this.child0,
            this.child1,
            this.child11,
            this.child12,
            this.child13,
            // this.child2,
            // this.child21,
            // this.child22,
            this.child3,
            this.child31,
        ];

        var expected = shadowNodes
            .Concat(lightNodes)
            .Select(x  => x.Name)
            .ToArray();

        var actual = new List<string>(expected.Length);

        // foreach (var node in Node.TraverseShadowTreeEnumerator.Traverse(host))
        // {
        //     actual.Add(node.Name!);
        // }

        // Assert.Equal(expected, actual);

        // actual.Clear();

        // foreach (var node in Node.TraverseShadowTreeEnumerator.TraverseNonRecursive(host))
        // {
        //     actual.Add(node.Name!);
        // }

        var enumerator = new Node.TraverseShadowTreeEnumerator(this.host);

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == this.child2 || enumerator.Current is Slot)
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
