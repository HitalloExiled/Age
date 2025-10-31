using Age.Commands;
using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public class RenderableTest
{
    private record TestCommand(string Name) : Command2D
    {
        public override string ToString() => this.Name;
    }

    private static void AssertIt(Renderable<Command2D>.SplitList list, ReadOnlySpan<Command2D> values, int capacity)
    {
        Assert.Equal(capacity, list.Capacity);
        Assert.Equal(values.Length, list.Count);

        Assert.True(list.AsSpan().SequenceEqual(values));
    }

    private static void AssertIt(Renderable<Command2D>.SplitList list, ReadOnlySpan<Command2D> pre, ReadOnlySpan<Command2D> post, int separator)
    {
        Assert.Equal(separator, list.Separator);
        Assert.True(list.Pre.SequenceEqual(pre));
        Assert.True(list.Post.SequenceEqual(post));
    }

    [Fact]
    public void SplitList()
    {
        var splitList = new Renderable<Command2D>.SplitList();

        var c1 = new TestCommand("c1");
        var c2 = new TestCommand("c2");
        var c3 = new TestCommand("c3");

        splitList.Add(c1);
        splitList.Add(c2);
        splitList.Add(c3);

        AssertIt(splitList, [c1, c2, c3], 4);

        splitList.Remove(c2);

        AssertIt(splitList, [c1, c3], 4);

        splitList.Insert(1, c2);

        AssertIt(splitList, [c1, c2, c3], 4);

        splitList.Add(c1);
        splitList.Add(c2);
        splitList.Add(c3);

        AssertIt(splitList, [c1, c2, c3, c1, c2, c3], 8);
        AssertIt(splitList, [c1, c2, c3, c1, c2, c3], [], -1);

        var span = splitList.AsSpan();

        splitList.RemoveAt(0);

        Assert.Equal([c2, c3, c1, c2, c3, default!], span);

        splitList.RemoveAt(2);

        Assert.Equal([c2, c3, c2, c3, default!, default!], span);
    }

    [Fact]
    public void AddPreAndPost()
    {
        var splitList = new Renderable<Command2D>.SplitList();

        var c1 = new TestCommand("c1");
        var c2 = new TestCommand("c2");
        var c3 = new TestCommand("c3");
        var c4 = new TestCommand("c4");
        var c5 = new TestCommand("c5");
        var c6 = new TestCommand("c6");

        splitList.AddPre(c1);

        AssertIt(splitList, [c1], [], 1);

        splitList.AddPre(c2);

        AssertIt(splitList, [c1, c2], [], 2);

        splitList.AddPre(c3);

        AssertIt(splitList, [c1, c2, c3], [], 3);

        splitList.AddPost(c4);

        AssertIt(splitList, [c1, c2, c3], [c4], 3);

        splitList.AddPost(c5);

        AssertIt(splitList, [c1, c2, c3], [c4, c5], 3);

        splitList.AddPost(c6);

        AssertIt(splitList, [c1, c2, c3], [c4, c5, c6], 3);
    }

    [Fact]
    public void AddPostAndPre()
    {
        var splitList = new Renderable<Command2D>.SplitList();

        var c1 = new TestCommand("c1");
        var c2 = new TestCommand("c2");
        var c3 = new TestCommand("c3");
        var c4 = new TestCommand("c4");
        var c5 = new TestCommand("c5");
        var c6 = new TestCommand("c6");

        splitList.AddPost(c4);

        AssertIt(splitList, [], [c4], 0);

        splitList.AddPost(c5);

        AssertIt(splitList, [], [c4, c5], 0);

        splitList.AddPost(c6);

        AssertIt(splitList, [], [c4, c5, c6], 0);

        splitList.AddPre(c1);

        AssertIt(splitList, [c1], [c4, c5, c6], 1);

        splitList.AddPre(c2);

        AssertIt(splitList, [c1, c2], [c4, c5, c6], 2);

        splitList.AddPre(c3);

        AssertIt(splitList, [c1, c2, c3], [c4, c5, c6], 3);
    }

    [Fact]
    public void SplitListWithSeparator()
    {
        var splitList = new Renderable<Command2D>.SplitList();

        var c1 = new TestCommand("c1");
        var c2 = new TestCommand("c2");
        var c3 = new TestCommand("c3");
        var c4 = new TestCommand("c4");
        var c5 = new TestCommand("c5");
        var c6 = new TestCommand("c6");

        splitList.Add(c1);
        splitList.Add(c2);
        splitList.Add(c3);
        splitList.Add(c4);
        splitList.Add(c5);
        splitList.Add(c6);

        splitList.Separator = 3;

        AssertIt(splitList, [c1, c2, c3], [c4, c5, c6], 3);

        splitList.Insert(2, c4);

        AssertIt(splitList, [c1, c2, c4, c3], [c4, c5, c6], 4);

        splitList.Insert(5, c5);

        AssertIt(splitList, [c1, c2, c4, c3], [c4, c5, c5, c6], 4);

        splitList.RemoveAt(5);

        AssertIt(splitList, [c1, c2, c4, c3], [c4, c5, c6], 4);

        splitList.RemoveAt(3);

        AssertIt(splitList, [c1, c2, c4], [c4, c5, c6], 3);
    }
}
