using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public class TestNode : Node
{
    public override string NodeName => nameof(TestNode);

    public TestNode(string name) =>
        this.Name = name;
}

public class HostTestNode : Node
{
    public override string NodeName => nameof(HostTestNode);

    public HostTestNode(string name, int depth = 2, int children = 2)
    {
        this.Name = name;

        var shadowRoot = TreeFactory.Linear<TestNode>(static name => new(name), depth, children, $"{name}.#");

        this.AttachShadowRoot(shadowRoot);
    }
}

public class NestedHostTestNode : Node
{
    public override string NodeName => nameof(HostTestNode);

    public NestedHostTestNode(string name, int depth = 1)
    {
        this.Name = name;

        var shadowRoot = TreeFactory.Linear<TestNode>(name => new(name), 2, 1, $"{name}.#");

        if (depth > 0)
        {
            var tail = shadowRoot.FirstChild!.FirstChild!;

            var nested = new NestedHostTestNode($"{tail.Name}.1", --depth);

            tail.AppendChild(nested);
        }

        this.AttachShadowRoot(shadowRoot);
    }
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
        var parent = new TestNode("parent");
        var child1 = new TestNode("child1");
        var child2 = new TestNode("child2");
        var child3 = new TestNode("child3");

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
        var parent = new TestNode("parent");
        var child1 = new TestNode("child1");
        var child2 = new TestNode("child2");
        var child3 = new TestNode("child3");
        var child4 = new TestNode("child4");
        var child5 = new TestNode("child5");

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
        var parent = new TestNode("parent");
        var child1 = new TestNode("child1");
        var child2 = new TestNode("child2");
        var child3 = new TestNode("child3");

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
        var parent = new TestNode("parent");
        var child1 = new TestNode("child1");
        var child2 = new TestNode("child2");
        var child3 = new TestNode("child3");
        var child4 = new TestNode("child4");
        var child5 = new TestNode("child5");

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
        var parent = new TestNode("parent");
        var child1 = new TestNode("child1");
        var child2 = new TestNode("child2");
        var child3 = new TestNode("child3");

        AssertParentHasNodes(parent, []);

        parent.AppendChild(child1);

        AssertParentHasNodes(parent, [child1]);

        parent.DetachChild(child1);

        AssertParentHasNodes(parent, []);

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child3);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.DetachChild(child1);

        AssertParentHasNodes(parent, [child2, child3]);

        parent.PrependChild(child1);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.DetachChild(child2);

        AssertParentHasNodes(parent, [child1, child3]);

        parent.InsertAfter(child1, child2);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.DetachChild(child3);

        AssertParentHasNodes(parent, [child1, child2]);

        parent.DetachChild(child1);

        AssertParentHasNodes(parent, [child2]);

        parent.DetachChild(child2);

        AssertParentHasNodes(parent, []);
    }

    [Fact]
    public void RemoveChildren()
    {
        var parent = new TestNode("parent");
        var child1 = new TestNode("child1");
        var child2 = new TestNode("child2");
        var child3 = new TestNode("child3");

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child3);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.DetachChildren();

        AssertParentHasNodes(parent, []);
    }

    [Fact]
    public void RemoveChildrenInRange()
    {
        var parent = new TestNode("parent");
        var child1 = new TestNode("child1");
        var child2 = new TestNode("child2");
        var child3 = new TestNode("child3");
        var child4 = new TestNode("child4");
        var child5 = new TestNode("child5");
        var child6 = new TestNode("child6");
        var child7 = new TestNode("child7");
        var child8 = new TestNode("child8");
        var child9 = new TestNode("child9");

        void appendAll() => parent.AppendChildren([child1, child2, child3, child4, child5, child6, child7, child8, child9]);

        appendAll();

        parent.DetachChildrenInRange(child1, child1);

        AssertParentHasNodes(parent, [child2, child3, child4, child5, child6, child7, child8, child9]);

        parent.DetachChildrenInRange(child2, child3);

        AssertParentHasNodes(parent, [child4, child5, child6, child7, child8, child9]);

        parent.DetachChildrenInRange(child4, child6);

        AssertParentHasNodes(parent, [child7, child8, child9]);

        parent.DetachChildrenInRange(child7, child9);

        AssertParentHasNodes(parent, []);

        appendAll();

        parent.DetachChildrenInRange(child9, child9);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7, child8]);

        parent.DetachChildrenInRange(child8, child7);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6]);

        parent.DetachChildrenInRange(child6, child4);

        AssertParentHasNodes(parent, [child1, child2, child3]);

        parent.DetachChildrenInRange(child3, child1);

        AssertParentHasNodes(parent, []);

        appendAll();

        parent.DetachChildrenInRange(child3, child7);

        AssertParentHasNodes(parent, [child1, child2, child8, child9]);

        appendAll();

        parent.DetachChildrenInRange(child1, child9);

        AssertParentHasNodes(parent, []);
    }

    [Fact]
    public void InsertBefore()
    {
        var parent = new TestNode("parent");
        var child0 = new TestNode("child.0");
        var child1 = new TestNode("child.1");
        var child2 = new TestNode("child.2");
        var child3 = new TestNode("child.3");
        var child4 = new TestNode("child.4");

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.AppendChild(child4);

        parent.InsertBefore(child4, child3);

        AssertParentHasNodes(parent, [child1, child2, child3, child4]);

        parent.InsertBefore(child1, child0);

        AssertParentHasNodes(parent, [child0, child1, child2, child3, child4]);
    }

    [Fact]
    public void InsertAfter()
    {
        var parent = new TestNode("parent");
        var child1 = new TestNode("child.1");
        var child2 = new TestNode("child.2");
        var child3 = new TestNode("child.3");
        var child4 = new TestNode("child.4");
        var child5 = new TestNode("child.5");

        parent.AppendChildren([child1, child3, child4]);

        parent.InsertAfter(child1, child2);

        AssertParentHasNodes(parent, [child1, child2, child3, child4]);

        parent.InsertAfter(child4, child5);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5]);
    }

    [Fact]
    public void InsertNodesBefore()
    {
        var parent = new TestNode("parent");
        var child1 = new TestNode("child.1");
        var child2 = new TestNode("child.2");
        var child3 = new TestNode("child.3");
        var child4 = new TestNode("child.4");
        var child5 = new TestNode("child.5");
        var child6 = new TestNode("child.6");
        var child7 = new TestNode("child.7");
        var child8 = new TestNode("child.8");
        var child9 = new TestNode("child.9");

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
        var parent = new TestNode("parent");
        var child1 = new TestNode("child.1");
        var child2 = new TestNode("child.2");
        var child3 = new TestNode("child.3");
        var child4 = new TestNode("child.4");
        var child5 = new TestNode("child.5");
        var child6 = new TestNode("child.6");
        var child7 = new TestNode("child.7");
        var child8 = new TestNode("child.8");
        var child9 = new TestNode("child.9");

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
        var parent = new TestNode("parent");
        var child0 = new TestNode("child.0");
        var child1 = new TestNode("child.1");
        var child2 = new TestNode("child.2");
        var child3 = new TestNode("child.3");

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
        var parent  = new TestNode("parent");
        var child0  = new TestNode("child.0");
        var child1  = new TestNode("child.1");
        var child2  = new TestNode("child.2");
        var child3  = new TestNode("child.3");
        var child4  = new TestNode("child.4");
        var child5  = new TestNode("child.5");
        var child6  = new TestNode("child.6");
        var child7  = new TestNode("child.7");
        var child8  = new TestNode("child.8");
        var child9  = new TestNode("child.9");
        var child10 = new TestNode("child.10");
        var child11 = new TestNode("child.11");
        var child12 = new TestNode("child.12");

        parent.AppendChild(child5);
        parent.AppendChild(child0);
        parent.AppendChild(child9);

        parent.ReplaceWith(child0, [child6, child7, child8]);

        AssertParentHasNodes(parent, [child5, child6, child7, child8, child9]);

        parent.InsertBefore(child5, child0);

        parent.ReplaceWith(child0, [child1, child2, child3, child4]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7, child8, child9]);

        parent.AppendChild(child0);

        parent.ReplaceWith(child0, [child10, child11, child12]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7, child8, child9, child10, child11, child12]);

        parent.DetachChildren();

        AssertParentHasNodes(parent, []);

        parent.AppendChildren([child1, child0, child7, child2, child3, child4, child5, child6]);

        parent.ReplaceWith(child0, [child2, child3, child4, child5, child6, child7]);

        AssertParentHasNodes(parent, [child1, child2, child3, child4, child5, child6, child7]);
    }

    [Fact]
    public void Enumerate()
    {
        var child1 = new TestNode("$.1");
        var child2 = new TestNode("$.2");
        var child3 = new TestNode("$.3");

        var parent = new TestNode("$");

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
    public void GetCommonAncestor()
    {
        var tree  = TreeFactory.Linear<TestNode>(static name => new(name), 3, 3);
        var nodes = TreeFactory.Flatten(tree).ToDictionary(static x => x.Name!, static x => x);

        tree.Connect();

        Assert.Equal(nodes["$.1"],   Node.GetCommonAncestor(nodes["$.1"],     nodes["$.1.1.1"]));
        Assert.Equal(nodes["$.1.2"], Node.GetCommonAncestor(nodes["$.1.2.1"], nodes["$.1.2.2"]));
        Assert.Equal(nodes["$"],     Node.GetCommonAncestor(nodes["$.2.2.1"], nodes["$.1.2"]));
        Assert.Equal(nodes["$"],     Node.GetCommonAncestor(nodes["$.2.1.3"], nodes["$.3.2.1"]));
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

        var root   = new TestNode("root");
        var a      = new TestNode("a");
        var aa     = new TestNode("a.a");
        var b      = new TestNode("b");
        var ba     = new TestNode("b.a");
        var baa    = new TestNode("b.a.a");
        var baaa   = new TestNode("b.a.a.a");
        var baab   = new TestNode("b.a.a.b");
        var baac   = new TestNode("b.a.a.c");
        var baaba  = new TestNode("b.a.a.b.a");
        var baabb  = new TestNode("b.a.a.b.b");
        var bab    = new TestNode("b.a.b");
        var bac    = new TestNode("b.a.c");
        var bb     = new TestNode("b.b");
        var bba    = new TestNode("b.b.a");
        var bbaa   = new TestNode("b.b.a.a");
        var bbab   = new TestNode("b.b.a.b");
        var bbac   = new TestNode("b.b.a.c");
        var bbaca  = new TestNode("b.b.a.c.a");
        var bbacb  = new TestNode("b.b.a.c.b");
        var bbacba = new TestNode("b.b.a.c.b.a");
        var bbacbb = new TestNode("b.b.a.c.b.b");
        var bbacc  = new TestNode("b.b.a.c.c");
        var bbb    = new TestNode("b.b.b");
        var bc     = new TestNode("b.c");
        var bca    = new TestNode("b.c.a");
        var bcb    = new TestNode("b.c.b");
        var c      = new TestNode("c");

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

        root.Connect();
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
    public void Depth()
    {
        var root = TreeFactory.Linear<TestNode>(static name => new(name), 5, 1);
        var flat = TreeFactory.Flatten(root);

        for (var i = 0; i < flat.Length; i++)
        {
            Assert.Equal(0, flat[i].Depth);
            Assert.Equal(0, flat[i].CompositeDepth);
        }

        root.Connect();

        for (var i = 0; i < flat.Length; i++)
        {
            Assert.Equal(i, flat[i].Depth);
            Assert.Equal(i, flat[i].CompositeDepth);
        }

        root.Disconnect();

        for (var i = 0; i < flat.Length; i++)
        {
            Assert.Equal(0, flat[i].Depth);
            Assert.Equal(0, flat[i].CompositeDepth);
        }
    }

    [Fact]
    public void CompositeDepth()
    {
        var root  = new NestedHostTestNode("$", 2);
        var flat  = TreeFactory.Flatten(root);
        var nodes = flat.ToDictionary(static x => x.Name!, static x => x);

        for (var i = 0; i < flat.Length; i++)
        {
            Assert.Equal(0, flat[i].Depth);
            Assert.Equal(0, flat[i].CompositeDepth);
        }

        root.Connect();

        Assert.Equal(0, nodes["$"].Depth);
        Assert.Equal(0, nodes["$.#"].Depth);
        Assert.Equal(1, nodes["$.#.1"].Depth);
        Assert.Equal(2, nodes["$.#.1.1"].Depth);
        Assert.Equal(3, nodes["$.#.1.1.1"].Depth);
        Assert.Equal(0, nodes["$.#.1.1.1.#"].Depth);
        Assert.Equal(1, nodes["$.#.1.1.1.#.1"].Depth);
        Assert.Equal(2, nodes["$.#.1.1.1.#.1.1"].Depth);
        Assert.Equal(3, nodes["$.#.1.1.1.#.1.1.1"].Depth);
        Assert.Equal(0, nodes["$.#.1.1.1.#.1.1.1.#"].Depth);
        Assert.Equal(1, nodes["$.#.1.1.1.#.1.1.1.#.1"].Depth);
        Assert.Equal(2, nodes["$.#.1.1.1.#.1.1.1.#.1.1"].Depth);

        Assert.Equal(0,  nodes["$"].CompositeDepth);
        Assert.Equal(1,  nodes["$.#"].CompositeDepth);
        Assert.Equal(2,  nodes["$.#.1"].CompositeDepth);
        Assert.Equal(3,  nodes["$.#.1.1"].CompositeDepth);
        Assert.Equal(4,  nodes["$.#.1.1.1"].CompositeDepth);
        Assert.Equal(5,  nodes["$.#.1.1.1.#"].CompositeDepth);
        Assert.Equal(6,  nodes["$.#.1.1.1.#.1"].CompositeDepth);
        Assert.Equal(7,  nodes["$.#.1.1.1.#.1.1"].CompositeDepth);
        Assert.Equal(8,  nodes["$.#.1.1.1.#.1.1.1"].CompositeDepth);
        Assert.Equal(9,  nodes["$.#.1.1.1.#.1.1.1.#"].CompositeDepth);
        Assert.Equal(10, nodes["$.#.1.1.1.#.1.1.1.#.1"].CompositeDepth);
        Assert.Equal(11, nodes["$.#.1.1.1.#.1.1.1.#.1.1"].CompositeDepth);

        root.Disconnect();

        for (var i = 0; i < flat.Length; i++)
        {
            Assert.Equal(0, flat[i].Depth);
            Assert.Equal(0, flat[i].CompositeDepth);
        }
    }

    [Fact]
    public void Compare()
    {
        var tree = TreeFactory.Linear<TestNode>(static name => new(name), [2, 2, 5]);

        var node3  = new HostTestNode("$.3");
        var node31 = TreeFactory.Linear<TestNode>(static name => new(name), 2, 1, "$.3.1");

        tree.AppendChild(node3);
        node3.AppendChild(node31);

        var nodes = TreeFactory.Flatten(tree).ToDictionary(static x => x.Name!, static x => x);

        tree.Connect();

        Assert.Equal(-1, nodes["$"].CompareTo(nodes["$.1"]));
        Assert.Equal(1,  nodes["$.1.1"].CompareTo(nodes["$"]));

        Assert.Equal(1,  nodes["$.1.1.3"].CompareTo(nodes["$.1.1.1"]));
        Assert.Equal(1,  nodes["$.1.1.3"].CompareTo(nodes["$.1.1.2"]));
        Assert.Equal(0,  nodes["$.1.1.3"].CompareTo(nodes["$.1.1.3"]));
        Assert.Equal(-1, nodes["$.1.1.3"].CompareTo(nodes["$.1.1.4"]));
        Assert.Equal(-1, nodes["$.1.1.3"].CompareTo(nodes["$.1.1.5"]));

        Assert.Equal(-1, nodes["$.1"].CompareTo(nodes["$.2.1"]));
        Assert.Equal(1,  nodes["$.2.2"].CompareTo(nodes["$.1.1"]));
        Assert.Equal(-1, nodes["$.1.1"].CompareTo(nodes["$.2"]));

        Assert.Equal(-1, nodes["$.3"].CompareTo(nodes["$.3.#.1"]));
        Assert.Equal(-1, nodes["$.3.#.2"].CompareTo(nodes["$.3.1"]));
        Assert.Equal(1,  nodes["$.3.1"].CompareTo(nodes["$.3.#.1.1"]));
    }

    [Fact]
    public void GetPathToCommonComposedAncestor()
    {
        var treeA  = TreeFactory.Linear<TestNode>(static name => new(name), [2, 2, 1, 1]);
        var nodesA = TreeFactory.Flatten(treeA).ToDictionary(x => x.Name!, x => x);

        var treeB  = TreeFactory.Linear<TestNode>(static name => new(name), [2, 2, 1, 1]);
        var nodesB = TreeFactory.Flatten(treeB).ToDictionary(x => x.Name!, x => x);

        treeA.Connect();
        treeB.Connect();

        var actual = Node.GetCompositePathBetween(nodesA["$"], nodesA["$.1"]);

        assert([nodesA["$"], nodesA["$.1"]], [nodesA["$"]], [nodesA["$.1"], nodesA["$"]], actual);

        actual = Node.GetCompositePathBetween(nodesA["$.1"], nodesA["$.2"]);

        assert([nodesA["$.1"], nodesA["$"], nodesA["$.2"]], [nodesA["$.1"], nodesA["$"]], [nodesA["$.2"], nodesA["$"]], actual);

        actual = Node.GetCompositePathBetween(nodesA["$.1"], nodesA["$.2.1.1"]);

        assert(
            [nodesA["$.1"], nodesA["$"], nodesA["$.2"], nodesA["$.2.1"], nodesA["$.2.1.1"]],
            [nodesA["$.1"], nodesA["$"]],
            [nodesA["$.2.1.1"], nodesA["$.2.1"], nodesA["$.2"], nodesA["$"]],
            actual
        );

        actual = Node.GetCompositePathBetween(nodesA["$.2.2.1.1"], nodesA["$.2.1.1"]);

        assert(
            [nodesA["$.2.2.1.1"], nodesA["$.2.2.1"], nodesA["$.2.2"], nodesA["$.2"], nodesA["$.2.1"], nodesA["$.2.1.1"]],
            [nodesA["$.2.2.1.1"], nodesA["$.2.2.1"], nodesA["$.2.2"], nodesA["$.2"]],
            [nodesA["$.2.1.1"], nodesA["$.2.1"], nodesA["$.2"]],
            actual
        );

        Assert.Throws<InvalidOperationException>(() => Node.GetCompositePathBetween(nodesA["$.2.2.1.1"], nodesB["$.2.2.1.1"]));

        static void assert(Node[] expectedPath, Node[] leftToAncestor, Node[] ancestorToRight, in ComposedPath actual)
        {
            var actualPath = actual.GetElements();

            Assert.Equal(expectedPath, actualPath.ToArray());
            Assert.Equal(leftToAncestor, actual.LeftToAncestor);
            Assert.Equal(ancestorToRight, actual.RightToAncestor);
        }
    }
}
