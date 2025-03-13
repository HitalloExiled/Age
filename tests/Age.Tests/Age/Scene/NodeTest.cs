using Age.Scene;

namespace Age.Tests.Age.Scene;

public class TestNode : Node
{
    public override string NodeName { get; } = nameof(TestNode);
}

public class NodeTest
{
    private static void AssertParentHasNodes(Node parent, Node[] nodes)
    {
        if (nodes.Length == 0)
        {
            Assert.Null(parent.FirstChild);
            Assert.Null(parent.LastChild);
        }
        else
        {
            if (nodes.Length == 1)
            {
                Assert.Equal(parent, nodes[0].Parent);

                Assert.Equal(nodes[0], parent.FirstChild);
                Assert.Equal(nodes[0], parent.LastChild);

                Assert.Null(nodes[0].PreviousSibling);
                Assert.Null(nodes[0].NextSibling);
            }
            else
            {
                var first = nodes[0];
                var last  = nodes[^1];

                Assert.Equal(parent, first.Parent);
                Assert.Equal(parent, last.Parent);

                Assert.Equal(first, parent.FirstChild);
                Assert.Equal(last, parent.LastChild);

                Assert.Null(first.PreviousSibling);
                Assert.Equal(first.NextSibling, nodes[1]);

                for (var i = 1; i < nodes.Length - 1; i++)
                {
                    Assert.Equal(nodes[i].PreviousSibling, nodes[i - 1]);
                    Assert.Equal(nodes[i].NextSibling, nodes[i + 1]);
                }

                Assert.Equal(last.PreviousSibling, nodes[^2]);
                Assert.Null(last.NextSibling);

                Assert.True(parent.Children.SequenceEqual(nodes));
            }
        }
    }

    [Fact]
    public void AppendChild()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child1" };
        var child2 = new TestNode { Name = "child2" };
        var child3 = new TestNode { Name = "child3" };

        Assert.Null(parent.FirstChild);
        Assert.Null(parent.LastChild);

        parent.AppendChild(child1);

        AssertParentHasNodes(parent, [child1]);

        parent.AppendChild(child2);

        AssertParentHasNodes(parent, [child1, child2]);

        parent.AppendChild(child3);

        AssertParentHasNodes(parent, [child1, child2, child3]);
    }

    [Fact]
    public void AppendChildren()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child1" };
        var child2 = new TestNode { Name = "child2" };
        var child3 = new TestNode { Name = "child3" };
        var child4 = new TestNode { Name = "child4" };
        var child5 = new TestNode { Name = "child5" };

        parent.AppendChildren([child1, child2]);

        AssertParentHasNodes(parent, [child1, child2]);

        parent.AppendChildren([child3, child4, child5]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5]);

        parent.AppendChildren([child3, child2, child1]);

        AssertParentHasNodes(parent, [child4, child5, child3, child2, child1]);
    }

    [Fact]
    public void PrependChild()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child1" };
        var child2 = new TestNode { Name = "child2" };
        var child3 = new TestNode { Name = "child3" };

        parent.PrependChild(child3);

        AssertParentHasNodes(parent, [child3]);

        parent.PrependChild(child2);

        AssertParentHasNodes(parent, [child2, child3]);

        parent.PrependChild(child1);

        AssertParentHasNodes(parent, [child1, child2, child3]);
    }

    [Fact]
    public void PrependChildren()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child1" };
        var child2 = new TestNode { Name = "child2" };
        var child3 = new TestNode { Name = "child3" };
        var child4 = new TestNode { Name = "child4" };
        var child5 = new TestNode { Name = "child5" };

        parent.PrependChildren([child4, child5]);

        AssertParentHasNodes(parent, [child4, child5]);

        parent.PrependChildren([child1, child2, child3]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5]);

        parent.PrependChildren([child5, child4, child3]);

        AssertParentHasNodes(parent, [child5, child4, child3, child1, child2]);
    }

    [Fact]
    public void RemoveChild()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child1" };
        var child2 = new TestNode { Name = "child2" };
        var child3 = new TestNode { Name = "child3" };

        AssertParentHasNodes(parent, []);

        parent.AppendChild(child1);

        AssertParentHasNodes(parent, [child1]);

        parent.RemoveChild(child1);

        AssertParentHasNodes(parent, []);

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child3);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.RemoveChild(child1);

        AssertParentHasNodes(parent, [child2, child3]);

        parent.PrependChild(child1);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.RemoveChild(child2);

        AssertParentHasNodes(parent, [child1, child3]);

        parent.InsertAfter(child1, child2);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.RemoveChild(child3);

        AssertParentHasNodes(parent, [child1, child2]);

        parent.RemoveChild(child1);

        AssertParentHasNodes(parent, [child2]);

        parent.RemoveChild(child2);

        AssertParentHasNodes(parent, []);
    }

    [Fact]
    public void RemoveChildren()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child1" };
        var child2 = new TestNode { Name = "child2" };
        var child3 = new TestNode { Name = "child3" };

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child3);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.RemoveChildren();

        AssertParentHasNodes(parent, []);
    }

    [Fact]
    public void RemoveChildrenInRange()
    {
        var parent = new TestNode { Name = "parent" };
        var child1 = new TestNode { Name = "child1" };
        var child2 = new TestNode { Name = "child2" };
        var child3 = new TestNode { Name = "child3" };
        var child4 = new TestNode { Name = "child4" };
        var child5 = new TestNode { Name = "child5" };
        var child6 = new TestNode { Name = "child6" };
        var child7 = new TestNode { Name = "child7" };
        var child8 = new TestNode { Name = "child8" };
        var child9 = new TestNode { Name = "child9" };

        void appendAll() => parent.AppendChildren([child1, child2, child3, child4, child5, child6, child7, child8, child9]);

        appendAll();

        parent.RemoveChildrenInRange(child1, child1);

        AssertParentHasNodes(parent, [child2, child3, child4, child5, child6, child7, child8, child9]);

        parent.RemoveChildrenInRange(child2, child3);

        AssertParentHasNodes(parent, [child4, child5, child6, child7, child8, child9]);

        parent.RemoveChildrenInRange(child4, child6);

        AssertParentHasNodes(parent, [child7, child8, child9]);

        parent.RemoveChildrenInRange(child7, child9);

        AssertParentHasNodes(parent, []);

        appendAll();

        parent.RemoveChildrenInRange(child9, child9);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7, child8]);

        parent.RemoveChildrenInRange(child8, child7);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6]);

        parent.RemoveChildrenInRange(child6, child4);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.RemoveChildrenInRange(child3, child1);

        AssertParentHasNodes(parent, []);

        appendAll();

        parent.RemoveChildrenInRange(child3, child7);

        AssertParentHasNodes(parent, [child1, child2, child8, child9]);

        appendAll();

        parent.RemoveChildrenInRange(child1, child9);

        AssertParentHasNodes(parent, []);
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

        AssertParentHasNodes(parent, [child1, child2, child3, child4]);

        parent.InsertBefore(child0, child1);

        AssertParentHasNodes(parent, [child0, child1, child2, child3, child4]);
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

        AssertParentHasNodes(parent, [child1, child2, child3, child4]);

        parent.InsertAfter(child4, child5);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5]);
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

        parent.InsertNodesBefore([child3], child9);

        AssertParentHasNodes(parent, [child1, child2, child3, child9]);

        parent.InsertNodesBefore([child4, child5], child9);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child9]);

        parent.InsertNodesBefore([child6, child7, child8], child9);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7, child8, child9]);

        parent.InsertNodesBefore([child1, child3, child5], child6);

        AssertParentHasNodes(parent, [child2, child4, child1, child3, child5, child6, child7, child8, child9]);
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

        parent.InsertNodesAfter(child2, [child3]);

        AssertParentHasNodes(parent, [child1, child2, child3, child9]);

        parent.InsertNodesAfter(child3, [child4, child5]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child9]);

        parent.InsertNodesAfter(child5, [child6, child7, child8]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7, child8, child9]);

        parent.InsertNodesAfter(child5, [child4, child3, child2]);

        AssertParentHasNodes(parent, [child1, child5, child4, child3, child2, child6, child7, child8, child9]);
    }

    [Fact]
    public void ReplaceNode()
    {
        var parent = new TestNode { Name = "parent" };
        var child0 = new TestNode { Name = "child.0" };
        var child1 = new TestNode { Name = "child.1" };
        var child2 = new TestNode { Name = "child.2" };
        var child3 = new TestNode { Name = "child.3" };

        parent.AppendChildren([child1, child0, child3]);

        parent.Replace(child0, child2);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.Replace(child3, child0);

        AssertParentHasNodes(parent, [child1, child2, child0]);

        parent.Replace(child1, child3);

        AssertParentHasNodes(parent, [child3, child2, child0]);

        parent.Replace(child0, child1);

        AssertParentHasNodes(parent, [child3, child2, child1]);
    }

    [Fact]
    public void ReplaceNodeWith()
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

        parent.ReplaceWith(child0, [child6, child7, child8]);

        AssertParentHasNodes(parent, [child5, child6, child7, child8, child9]);

        parent.InsertBefore(child0, child5);

        parent.ReplaceWith(child0, [child1, child2, child3, child4]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7, child8, child9]);

        parent.AppendChild(child0);

        parent.ReplaceWith(child0, [child10, child11, child12]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7, child8, child9, child10, child11, child12]);

        parent.RemoveChildren();

        AssertParentHasNodes(parent, []);

        parent.AppendChildren([child1, child0, child7, child2, child3, child4, child5, child6]);

        parent.ReplaceWith(child0, [child2, child3, child4, child5, child6, child7]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7]);
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

        foreach (var node in parent.GetTraversalEnumerator())
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

        var enumerator = new Node.TraversalEnumerator(parent);

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
