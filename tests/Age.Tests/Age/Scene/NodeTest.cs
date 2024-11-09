using Age.Scene;

namespace Age.Tests.Age.Scene;

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
            parent.AppendChild(child1);
            parent.AppendChild(child2);
            parent.AppendChild(child3);

            Assert.Equal(child1, parent.FirstChild);
            Assert.Equal(child3, parent.LastChild);
            Assert.True(parent.Children.SequenceEqual([child1, child2, child3]));
        }

        appendAll();

        parent.RemoveChild(child3);

        Assert.Null(child3.PreviousSibling);
        Assert.Null(child3.NextSibling);

        Assert.Equal(child1, parent.FirstChild);
        Assert.Equal(child2, parent.LastChild);
        Assert.True(parent.Children.SequenceEqual([child1, child2]));

        parent.RemoveChild(child2);

        Assert.Null(child2.PreviousSibling);
        Assert.Null(child2.NextSibling);

        Assert.Equal(child1, parent.FirstChild);
        Assert.Equal(child1, parent.LastChild);
        Assert.True(parent.Children.SequenceEqual([child1]));

        parent.RemoveChild(child1);

        Assert.Null(child1.PreviousSibling);
        Assert.Null(child1.NextSibling);

        Assert.Null(parent.FirstChild);
        Assert.Null(parent.LastChild);
        Assert.True(parent.Children.SequenceEqual([]));

        appendAll();

        parent.RemoveChild(child1);

        Assert.Null(child1.PreviousSibling);
        Assert.Null(child1.NextSibling);

        Assert.Equal(child2, parent.FirstChild);
        Assert.Equal(child3, parent.LastChild);
        Assert.True(parent.Children.SequenceEqual([child2, child3]));

        parent.RemoveChild(child2);

        Assert.Null(child2.PreviousSibling);
        Assert.Null(child2.NextSibling);

        Assert.Equal(child3, parent.FirstChild);
        Assert.Equal(child3, parent.LastChild);
        Assert.True(parent.Children.SequenceEqual([child3]));

        parent.RemoveChild(child3);

        Assert.Null(child3.PreviousSibling);
        Assert.Null(child3.NextSibling);

        Assert.Null(parent.FirstChild);
        Assert.Null(parent.LastChild);
        Assert.True(parent.Children.SequenceEqual([]));

        appendAll();

        parent.RemoveChild(child2);

        Assert.Null(child2.PreviousSibling);
        Assert.Null(child2.NextSibling);

        Assert.Equal(child1, parent.FirstChild);
        Assert.Equal(child3, parent.LastChild);
        Assert.True(parent.Children.SequenceEqual([child1, child3]));
    }

    [Fact]
    public void InsertBefore()
    {
        var parent = new TestNode { Name = "parent" };
        var child0 = new TestNode { Name = "child.0" };
        var child1 = new TestNode { Name = "child.1" };
        var child2 = new TestNode { Name = "child.2" };
        var child3 = new TestNode { Name = "child.3" };
        var child4 = new TestNode { Name = "child.4" };

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child4);

        parent.InsertBefore(child3, child4);

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4]));

        parent.InsertBefore(child0, child1);

        Assert.True(parent.Children.SequenceEqual([child0, child1, child2, child3, child4]));
    }

    [Fact]
    public void InsertAfter()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child.1" };
        var child2 = new TestNode { Name = "child.2" };
        var child3 = new TestNode { Name = "child.3" };
        var child4 = new TestNode { Name = "child.4" };
        var child5 = new TestNode { Name = "child.5" };

        parent.AppendChildren([child1, child3, child4]);

        parent.InsertAfter(child1, child2);

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4]));

        parent.InsertAfter(child4, child5);

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4, child5]));
    }

    [Fact]
    public void InsertNodesBefore()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child.1" };
        var child2 = new TestNode { Name = "child.2" };
        var child3 = new TestNode { Name = "child.3" };
        var child4 = new TestNode { Name = "child.4" };
        var child5 = new TestNode { Name = "child.5" };
        var child6 = new TestNode { Name = "child.6" };
        var child7 = new TestNode { Name = "child.7" };
        var child8 = new TestNode { Name = "child.8" };
        var child9 = new TestNode { Name = "child.9" };

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child9);

        parent.InsertBefore(new Node[] { child3 }, child9);

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child9]));

        parent.InsertBefore(new Node[] { child4, child5 }, child9);

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4, child5, child9]));

        parent.InsertBefore(new Node[] { child6, child7, child8 }, child9);

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4, child5, child6, child7, child8, child9]));
    }

    [Fact]
    public void InsertNodesAfter()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child.1" };
        var child2 = new TestNode { Name = "child.2" };
        var child3 = new TestNode { Name = "child.3" };
        var child4 = new TestNode { Name = "child.4" };
        var child5 = new TestNode { Name = "child.5" };
        var child6 = new TestNode { Name = "child.6" };
        var child7 = new TestNode { Name = "child.7" };
        var child8 = new TestNode { Name = "child.8" };
        var child9 = new TestNode { Name = "child.9" };

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child9);

        parent.InsertAfter(child2, new Node[] { child3 });

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child9]));

        parent.InsertAfter(child3, new Node[] { child4, child5 });

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4, child5, child9]));

        parent.InsertAfter(child5, new Node[] { child6, child7, child8 });

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4, child5, child6, child7, child8, child9]));
    }

    [Fact]
    public void Replace()
    {
        var parent = new TestNode { Name = "parent" };
        var child0 = new TestNode { Name = "child.0" };
        var child1 = new TestNode { Name = "child.1" };
        var child2 = new TestNode { Name = "child.2" };
        var child3 = new TestNode { Name = "child.3" };

        parent.AppendChildren([child1, child0, child3]);

        parent.Replace(child0, child2);

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3]));

        parent.Replace(child3, child0);

        Assert.True(parent.Children.SequenceEqual([child1, child2, child0]));

        parent.Replace(child1, child3);

        Assert.True(parent.Children.SequenceEqual([child3, child2, child0]));

        parent.Replace(child0, child1);

        Assert.True(parent.Children.SequenceEqual([child3, child2, child1]));
    }

    [Fact]
    public void ReplaceByNodes()
    {
        var parent  = new TestNode { Name = "parent" };
        var child0  = new TestNode { Name = "child.0" };
        var child1  = new TestNode { Name = "child.1" };
        var child2  = new TestNode { Name = "child.2" };
        var child3  = new TestNode { Name = "child.3" };
        var child4  = new TestNode { Name = "child.4" };
        var child5  = new TestNode { Name = "child.5" };
        var child6  = new TestNode { Name = "child.6" };
        var child7  = new TestNode { Name = "child.7" };
        var child8  = new TestNode { Name = "child.8" };
        var child9  = new TestNode { Name = "child.9" };
        var child10 = new TestNode { Name = "child.10" };
        var child11 = new TestNode { Name = "child.11" };
        var child12 = new TestNode { Name = "child.12" };

        parent.AppendChild(child5);
        parent.AppendChild(child0);
        parent.AppendChild(child9);

        parent.Replace(child0, new Node[] { child6, child7, child8 });

        Assert.True(parent.Children.SequenceEqual([child5, child6, child7, child8, child9]));

        parent.InsertBefore(child0, child5);

        parent.Replace(child0, new Node[] { child1, child2, child3, child4 });

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4, child5, child6, child7, child8, child9]));

        parent.AppendChild(child0);

        parent.Replace(child0, new Node[] { child10, child11, child12 });

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4, child5, child6, child7, child8, child9, child10, child11, child12]));
    }

    [Fact]
    public void ReplaceByNodesEdgeCase()
    {
        var parent  = new TestNode { Name = "parent" };
        var child0  = new TestNode { Name = "child.0" };
        var child1  = new TestNode { Name = "child.1" };
        var child2  = new TestNode { Name = "child.2" };
        var child3  = new TestNode { Name = "child.3" };
        var child4  = new TestNode { Name = "child.4" };
        var child5  = new TestNode { Name = "child.5" };
        var child6  = new TestNode { Name = "child.6" };
        var child7  = new TestNode { Name = "child.7" };

        parent.AppendChildren([child1, child0, child7, child2, child3, child4, child5, child6]);

        parent.Replace(child0, new Node[] { child2, child3, child4, child5, child6, child7 });

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3, child4, child5, child6, child7]));
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

        Assert.True(parent.Children.SequenceEqual([child1, child2, child3]));

        parent.RemoveChildren();

        Assert.Null(child1.PreviousSibling);
        Assert.Null(child1.NextSibling);

        Assert.Null(child2.PreviousSibling);
        Assert.Null(child2.NextSibling);

        Assert.Null(child3.PreviousSibling);
        Assert.Null(child3.NextSibling);

        Assert.True(parent.Children.SequenceEqual([]));
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

        Assert.True(nodes.SequenceEqual([child1, child2, child3]));
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
