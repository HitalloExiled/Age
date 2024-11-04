using Age.Scene;

namespace Age.Tests.Age.Drawing;

public class TestNode : Node
{
    public override string NodeName { get; } = nameof(TestNode);
}

public class NodeTest
{
    [Fact]
    public void AppendAndRemove()
    {
        var parent = new TestNode();
        var child1 = new TestNode();
        var child2 = new TestNode();
        var child3 = new TestNode();

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
        var parent = new TestNode();
        var child1 = new TestNode();
        var child2 = new TestNode();
        var child3 = new TestNode();

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
        var child1 = new TestNode();
        var child2 = new TestNode();
        var child3 = new TestNode();

        var parent = new TestNode();
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
    public void SelectBetween()
    {
        #region Arrange
        /*
                                                 [R]
                       ┌──────────────────────────┼──────┐
                      [A]                        [B]    [C]
                       │      ┌───────────────────┼───────────────┐
                     [A.A]  [B.A]               [B.B]           [B.C]
                      ┌───────┼───────┐       ┌───┴───┐       ┌───┴───┐
                   [B.A.A] (B.A.B) (B.A.C) [B.B.A] [B.B.B] [B.C.A] [B.C.B]
            ┌─────────┼─────────┐             │
        [B.A.A.A] [B.A.A.B] (B.A.A.C)         │
                ┌─────┴─────┐                 │
           [B.A.A.B.A] [B.A.A.B.B] <- Start   │
                                    ┌─────────┼─────────┐
                                (B.B.A.A) (B.B.A.B) [B.B.A.C]
                                                        │
                                            ┌───────────┼───────────┐
                                       (B.B.A.C.A) [B.B.A.C.B] [B.B.A.C.C]
                                                ┌───────┴───────┐
                                   End -> [B.B.A.C.B.A] [B.B.A.C.B.B]
        */

        var root   = new TestNode { Name = "root" };
        var a      = new TestNode { Name = "a" };
        var aa     = new TestNode { Name = "a.a" };
        var b      = new TestNode { Name = "b" };
        var ba     = new TestNode { Name = "b.a" };
        var baa    = new TestNode { Name = "b.a.a" };
        var baaa   = new TestNode { Name = "b.a.a.a" };
        var baab   = new TestNode { Name = "b.a.a.b" };
        var baac   = new TestNode { Name = "b.a.a.c" };
        var baaba  = new TestNode { Name = "b.a.a.b.a" };
        var baabb  = new TestNode { Name = "b.a.a.b.b" };
        var bab    = new TestNode { Name = "b.a.b" };
        var bac    = new TestNode { Name = "b.a.c" };
        var bb     = new TestNode { Name = "b.b" };
        var bba    = new TestNode { Name = "b.b.a" };
        var bbaa   = new TestNode { Name = "b.b.a.a" };
        var bbab   = new TestNode { Name = "b.b.a.b" };
        var bbac   = new TestNode { Name = "b.b.a.c" };
        var bbaca  = new TestNode { Name = "b.b.a.c.a" };
        var bbacb  = new TestNode { Name = "b.b.a.c.b" };
        var bbacba = new TestNode { Name = "b.b.a.c.b.a" };
        var bbacbb = new TestNode { Name = "b.b.a.c.b.b" };
        var bbacc  = new TestNode { Name = "b.b.a.c.c" };
        var bbb    = new TestNode { Name = "b.b.b" };
        var bc     = new TestNode { Name = "b.c" };
        var bca    = new TestNode { Name = "b.c.a" };
        var bcb    = new TestNode { Name = "b.c.b" };
        var c      = new TestNode { Name = "c" };

        var start = baabb;
        var end   = bbacba;

        root.AppendChild(a);
            a.AppendChild(aa);

        root.AppendChild(b);
            b.AppendChild(ba);
                ba.AppendChild(baa);
                    baa.AppendChild(baaa);
                    baa.AppendChild(baab);
                        baab.AppendChild(baaba);
                        baab.AppendChild(baabb);
                    baa.AppendChild(baac);
                ba.AppendChild(bab);
                ba.AppendChild(bac);
            b.AppendChild(bb);
                bb.AppendChild(bba);
                    bba.AppendChild(bbaa);
                    bba.AppendChild(bbab);
                    bba.AppendChild(bbac);
                        bbac.AppendChild(bbaca);
                        bbac.AppendChild(bbacb);
                            bbacb.AppendChild(bbacba);
                            bbacb.AppendChild(bbacbb);
                        bbac.AppendChild(bbacc);
                bb.AppendChild(bbb);
            b.AppendChild(bc);
                bc.AppendChild(bca);
                bc.AppendChild(bcb);
        root.AppendChild(c);
        #endregion

        var expected = new Node[]
        {
            baac,
            bab,
            bac,
            bbaa,
            bbab,
            bbaca,
        };

        Assert.Equal(expected, Node.SelectBetween(start, end));
        Assert.Equal(expected, Node.SelectBetween(end, start));
    }

    [Fact]
    public void Traverse()
    {
        var child11 = new TestNode { Name = "$.1.1" };
        var child12 = new TestNode { Name = "$.1.2" };
        var child13 = new TestNode { Name = "$.1.3" };

        var child21 = new TestNode { Name = "$.2.1" };
        var child22 = new TestNode { Name = "$.2.2" };

        var child31 = new TestNode { Name = "$.3.1" };

        var child0 = new TestNode { Name = "$.0" };

        var child1 = new TestNode { Name = "$.1" };
        child1.AppendChildren([child11, child12, child13]);

        var child2 = new TestNode { Name = "$.2" };
        child2.AppendChildren([child21, child22]);

        var child3 = new TestNode { Name = "$.3" };
        child3.AppendChild(child31);

        var parent = new TestNode { Name = "$" };
        parent.AppendChildren([child0, child1, child2, child3]);

        var nodes = new List<string>();

        var expected = new TestNode[]
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
        var child11 = new TestNode { Name = "$.1.1" };
        var child12 = new TestNode { Name = "$.1.2" };
        var child13 = new TestNode { Name = "$.1.3" };

        var child21 = new TestNode { Name = "$.2.1" };
        var child22 = new TestNode { Name = "$.2.2" };

        var child31 = new TestNode { Name = "$.3.1" };

        var child0 = new TestNode { Name = "$.0" };

        var child1 = new TestNode { Name = "$.1" };
        child1.AppendChildren([child11, child12, child13]);

        var child2 = new TestNode { Name = "$.2" };
        child2.AppendChildren([child21, child22]);

        var child3 = new TestNode { Name = "$.3" };
        child3.AppendChild(child31);

        var parent = new TestNode { Name = "$" };
        parent.AppendChildren([child0, child1, child2, child3]);

        var nodes = new List<string>();

        var enumerator = new Node.TraverseEnumerator(parent);

        var expected = new TestNode[]
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
