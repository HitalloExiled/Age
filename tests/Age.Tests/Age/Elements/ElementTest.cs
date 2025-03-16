using Age.Elements;

namespace Age.Tests.Age.Elements;

public class ElementTest
{
    public class HostTestElement : Element
    {
        public override string NodeName { get; } = nameof(HostTestElement);

        public HostTestElement(string name)
        {
            this.Name = name;
            this.AttachShadowTree();

            this.ShadowTree.Name = $"{name}.#";

            this.ShadowTree.Children =
            [
                new TestElement($"{name}.#.1")
                {
                    Children =
                    [
                        new TestElement($"{name}.#.1.1"),
                    ]
                },
                new TestElement($"{name}.#.2"),
            ];
        }
    }

    [Fact]
    public void ElementSibling()
    {
        var parent = new FlexBox();

        var text1 = new Text();
        var child1 = new FlexBox { Name = "child1" };
        var text2 = new Text();
        var child2 = new FlexBox { Name = "child2" };
        var text3 = new Text();
        var child3 = new FlexBox { Name = "child3" };
        var text4 = new Text();

        parent.AppendChild(text1);
        parent.AppendChild(child1);
        parent.AppendChild(text2);
        parent.AppendChild(child2);
        parent.AppendChild(text3);
        parent.AppendChild(child3);
        parent.AppendChild(text4);


        Assert.Null(parent.PreviousElementSibling);
        Assert.Equal(child1, parent.FirstElementChild);
        Assert.Equal(child1, child2.PreviousElementSibling);
        Assert.Equal(child3, child2.NextElementSibling);
        Assert.Equal(child3, parent.LastElementChild);
        Assert.Null(parent.NextElementSibling);
        Assert.Equal([child1, child2, child3], parent.Children);
    }

    [Fact]
    public void ElementSiblingSingleNode()
    {
        var parent = new FlexBox();

        var child1 = new FlexBox();

        parent.AppendChild(child1);

        Assert.Equal(child1, parent.FirstElementChild);
        Assert.Null(child1.PreviousElementSibling);
        Assert.Null(child1.NextElementSibling);
        Assert.Equal(child1, parent.LastElementChild);
        Assert.Equal([child1], parent.Children);
    }

    [Fact]
    public void Compare()
    {
        TestElement node1;
        TestElement node11;
        TestElement node111;
        TestElement node112;
        TestElement node113;
        TestElement node114;
        TestElement node115;
        TestElement node2;
        TestElement node21;
        TestElement node22;
        HostTestElement node3;
        TestElement node31;
        TestElement node3_1;
        TestElement node3_11;
        TestElement node3_2;

        var root = new TestElement("$")
        {
            Children =
            [
                node1 = new TestElement("$.1")
                {
                    Children =
                    [
                        node11 = new TestElement("$.1.1")
                        {
                            Children =
                            [
                                node111 = new TestElement("$.1.1.1"),
                                node112 = new TestElement("$.1.1.2"),
                                node113 = new TestElement("$.1.1.3"),
                                node114 = new TestElement("$.1.1.4"),
                                node115 = new TestElement("$.1.1.5"),
                            ]
                        },

                    ]
                },
                node2 = new TestElement("$.2")
                {
                    Children =
                    [
                        node21 = new TestElement("$.2.1"),
                        node22 = new TestElement("$.2.2"),
                    ],
                },
                node3 = new HostTestElement("$.3")
                {
                    Children =
                    [
                        node31 = new TestElement("$.3.1"),
                    ]
                }
            ]
        };

        node3_1  = (TestElement)node3.ShadowTree!.FirstChild!;
        node3_11 = (TestElement)node3_1.FirstChild!;
        node3_2  = (TestElement)node3_1.NextSibling!;

        Assert.Equal(-1, root.CompareTo(node1));
        Assert.Equal(1,  node11.CompareTo(root));

        Assert.Equal(1,  node113.CompareTo(node111));
        Assert.Equal(1,  node113.CompareTo(node112));
        Assert.Equal(0,  node113.CompareTo(node113));
        Assert.Equal(-1, node113.CompareTo(node114));
        Assert.Equal(-1, node113.CompareTo(node115));

        Assert.Equal(-1, node1.CompareTo(node21));
        Assert.Equal(1,  node22.CompareTo(node11));
        Assert.Equal(-1, node11.CompareTo(node2));

        Assert.Equal(-1, node3.CompareTo(node3_1));
        Assert.Equal(-1, node3_2.CompareTo(node31));
        Assert.Equal(1,  node31.CompareTo(node3_11));
    }
}
