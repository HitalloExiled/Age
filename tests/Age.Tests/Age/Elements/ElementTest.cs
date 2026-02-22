using Age.Elements;
using Age.Scenes;
using Xunit.Internal;

namespace Age.Tests.Age.Elements;

public class ElementTest
{
    public class EmptyNode : Node
    {
        public override string NodeName => nameof(EmptyNode);
    }

    [Fact]
    public void ElementSibling()
    {
        var parent = new FlexBox();

        var text1  = new EmptyNode();
        var child1 = new FlexBox { Name = "child1" };
        var text2  = new EmptyNode();
        var child2 = new FlexBox { Name = "child2" };
        var text3  = new EmptyNode();
        var child3 = new FlexBox { Name = "child3" };
        var text4  = new EmptyNode();

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
}
