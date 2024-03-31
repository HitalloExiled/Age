using Age.Rendering.Drawing.Elements;

namespace Age.Tests.Rendering.Drawing;

public class NodeTest
{
    [Fact]
    public void AppendAndRemove()
    {
        var parent = new Span();
        var child1 = new Span();
        var child2 = new Span();
        var child3 = new Span();

        void appendAll()
        {
            parent!.AppendChild(child1!);
            parent.AppendChild(child2!);
            parent.AppendChild(child3!);

            Assert.Equal(child1, parent.FirstChild);
            Assert.Equal(child3, parent.LastChild);
            Assert.Equal([child1, child2, child3], parent.Children);
        }

        appendAll();

        parent.RemoveChild(child3);

        Assert.Null(child3.PreviousSibling);
        Assert.Null(child3.NextSibling);

        Assert.Equal(child1, parent.FirstChild);
        Assert.Equal(child2, parent.LastChild);
        Assert.Equal([child1, child2], parent.Children);

        parent.RemoveChild(child2);

        Assert.Null(child2.PreviousSibling);
        Assert.Null(child2.NextSibling);

        Assert.Equal(child1, parent.FirstChild);
        Assert.Equal(child1, parent.LastChild);
        Assert.Equal([child1], parent.Children);

        parent.RemoveChild(child1);

        Assert.Null(child1.PreviousSibling);
        Assert.Null(child1.NextSibling);

        Assert.Null(parent.FirstChild);
        Assert.Null(parent.LastChild);
        Assert.Equal([], parent.Children);

        appendAll();

        parent.RemoveChild(child1);

        Assert.Null(child1.PreviousSibling);
        Assert.Null(child1.NextSibling);

        Assert.Equal(child2, parent.FirstChild);
        Assert.Equal(child3, parent.LastChild);
        Assert.Equal([child2, child3], parent.Children);

        parent.RemoveChild(child2);

        Assert.Null(child2.PreviousSibling);
        Assert.Null(child2.NextSibling);

        Assert.Equal(child3, parent.FirstChild);
        Assert.Equal(child3, parent.LastChild);
        Assert.Equal([child3], parent.Children);

        parent.RemoveChild(child3);

        Assert.Null(child3.PreviousSibling);
        Assert.Null(child3.NextSibling);

        Assert.Null(parent.FirstChild);
        Assert.Null(parent.LastChild);
        Assert.Equal([], parent.Children);

        appendAll();

        parent.RemoveChild(child2);

        Assert.Null(child2.PreviousSibling);
        Assert.Null(child2.NextSibling);

        Assert.Equal(child1, parent.FirstChild);
        Assert.Equal(child3, parent.LastChild);
        Assert.Equal([child1, child3], parent.Children);
    }

    [Fact]
    public void Clear()
    {
        var parent = new Span();
        var child1 = new Span();
        var child2 = new Span();
        var child3 = new Span();

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child3);

        Assert.Equal([child1, child2, child3], parent.Children);

        parent.RemoveChildren();

        Assert.Null(child1.PreviousSibling);
        Assert.Null(child1.NextSibling);

        Assert.Null(child2.PreviousSibling);
        Assert.Null(child2.NextSibling);

        Assert.Null(child3.PreviousSibling);
        Assert.Null(child3.NextSibling);

        Assert.Equal([], parent.Children);
    }
}
