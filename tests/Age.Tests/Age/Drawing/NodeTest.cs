using Age.Drawing.Elements;
using Age.Scene;

namespace Age.Tests.Age.Drawing;

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

    [Fact]
    public void Enumerate()
    {
        var child1 = new Span();
        var child2 = new Span();
        var child3 = new Span();

        var parent = new Span();
        parent.AppendChildren([child1, child2, child3]);

        var nodes = new List<Node>();

        var enumerator = new Node.Enumerator(parent);

        while (enumerator.MoveNext())
        {
            nodes.Add(enumerator.Current);
        }

        Assert.Equal([child1, child2, child3], nodes);
    }

    [Fact]
    public void Traverse()
    {
        var child11 = new Span { Name = "$.1.1" };
        var child12 = new Span { Name = "$.1.2" };
        var child13 = new Span { Name = "$.1.3" };

        var child21 = new Span { Name = "$.2.1" };
        var child22 = new Span { Name = "$.2.2" };

        var child31 = new Span { Name = "$.3.1" };

        var child0 = new Span { Name = "$.0" };

        var child1 = new Span { Name = "$.1" };
        child1.AppendChildren([child11, child12, child13]);

        var child2 = new Span { Name = "$.2" };
        child2.AppendChildren([child21, child22]);

        var child3 = new Span { Name = "$.3" };
        child3.AppendChild(child31);

        var parent = new Span { Name = "$" };
        parent.AppendChildren([child0, child1, child2, child3]);

        var nodes = new List<string>();

        var expected = new Span[]
        {
            child0,
            child1,
            child11,
            child12,
            child13,
            child2,
            child21,
            child22,
            child3,
            child31,
        }
         .Select(x  => x.Name)
         .ToArray();

        foreach (var node in parent.Traverse())
        {
            nodes.Add(node.Name!);
        }

        Assert.Equal(expected, nodes);
    }

    [Fact]
    public void TraverseTraverseEnumerator()
    {
        var child11 = new Span { Name = "$.1.1" };
        var child12 = new Span { Name = "$.1.2" };
        var child13 = new Span { Name = "$.1.3" };

        var child21 = new Span { Name = "$.2.1" };
        var child22 = new Span { Name = "$.2.2" };

        var child31 = new Span { Name = "$.3.1" };

        var child0 = new Span { Name = "$.0" };

        var child1 = new Span { Name = "$.1" };
        child1.AppendChildren([child11, child12, child13]);

        var child2 = new Span { Name = "$.2" };
        child2.AppendChildren([child21, child22]);

        var child3 = new Span { Name = "$.3" };
        child3.AppendChild(child31);

        var parent = new Span { Name = "$" };
        parent.AppendChildren([child0, child1, child2, child3]);

        var nodes = new List<string>();

        var enumerator = new Node.TraverseEnumerator(parent);

        var expected = new Span[]
        {
            child0,
            child1,
            child11,
            child12,
            child13,
            child2,
            child21,
            child22,
            child3,
            child31,
        }
         .Select(x  => x.Name)
         .ToArray();

        while (enumerator.MoveNext())
        {
            nodes.Add(enumerator.Current.Name!);
        }

        Assert.Equal(expected, nodes);
    }
}