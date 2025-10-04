using Age.Scenes;

namespace Age.Tests.Age.Scenes;

#pragma warning disable CA1001, IDE0052

public class TraversalEnumeratorTest
{
    private readonly TestNode grandParent;
    private readonly TestNode parent;
    private readonly TestNode child1;
    private readonly TestNode child111;
    private readonly TestNode child112;
    private readonly TestNode child113;
    private readonly TestNode child11;
    private readonly TestNode child121;
    private readonly TestNode child122;
    private readonly TestNode child12;
    private readonly TestNode child131;
    private readonly TestNode child13;

    public TraversalEnumeratorTest()
    {
        this.grandParent = new TestNode
        {
            Name     = "$$",
            Children =
            [
                this.parent = new TestNode
                {
                    Name     = "$",
                    Children =
                    [
                        this.child1 = new TestNode
                        {
                            Name     = "$.1",
                            Children =
                            [
                                this.child11 = new TestNode
                                {
                                    Name     = "$.1.1",
                                    Children =
                                    [
                                        this.child111 = new TestNode { Name = "$.1.1.1" },
                                        this.child112 = new TestNode { Name = "$.1.1.2" },
                                        this.child113 = new TestNode { Name = "$.1.1.3" },
                                    ],
                                },
                                this.child12 = new TestNode
                                {
                                    Name     = "$.1.2",
                                    Children =
                                    [
                                        this.child121 = new TestNode { Name = "$.1.2.1" },
                                        this.child122 = new TestNode { Name = "$.1.2.2" },
                                    ],
                                },
                                this.child13 = new TestNode
                                {
                                    Name     = "$.1.3",
                                    Children =
                                    [
                                        this.child131 = new TestNode { Name = "$.1.3.1" },
                                    ],
                                },
                            ],
                        },
                    ]
                },
                new TestNode { Name = "Ignored" },
            ]
        };
    }

    [Fact]
    public void Enumerate()
    {
        var nodes = new List<string>();

        var enumerator = new Node.TraversalEnumerator(this.parent);

        var expected = new TestNode[]
        {
            this.child1,
            this.child11,
            this.child111,
            this.child112,
            this.child113,
            this.child12,
            this.child121,
            this.child122,
            this.child13,
            this.child131,
        }
         .Select(x  => x.Name)
         .ToArray();

        while (enumerator.MoveNext())
        {
            nodes.Add(enumerator.Current.Name!);
        }

        Assert.Equal(expected, nodes);
    }

    [Fact]
    public void SkipToNextSibling()
    {
        var enumerator = new Node.TraversalEnumerator(this.parent);

        var expected = new TestNode[]
        {
            this.child1,
            this.child11,
            this.child111,
            this.child112,
            this.child113,
            // this.child12,
            // this.child121,
            // this.child122,
            this.child13,
            this.child131,
        }
        .Select(x  => x.Name)
        .ToArray();

        var actual = new List<string>();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == this.child12)
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
