using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class InlineList4Tests
{
    private static void AssertIt<T>(in InlineList4<T> list, ReadOnlySpan<T> expected)
    {
        Assert.Equal(expected.Length, list.Length);
        Assert.True(expected.SequenceEqual(list));
    }

    [Fact]
    public void AddAndRemove()
    {
        var list = new InlineList4<int>();

        list.Add(1);

        AssertIt(list, [1]);

        list.Add(2);

        AssertIt(list, [1, 2]);

        list.Add(3);

        AssertIt(list, [1, 2, 3]);

        list.Add(4);

        AssertIt(list, [1, 2, 3, 4]);

        list.RemoveAt(1, 2);

        AssertIt(list, [1, 4]);

        list.RemoveAt(1);

        AssertIt(list, [1]);

        list.Remove(1);

        AssertIt(list, []);
    }

    [Fact]
    public void Compare()
    {
        var a = new InlineList4<int>(1, 2, 3, 4);
        var b = new InlineList4<int>(1, 2, 3, 4);

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ThrowIfExceeds() =>
        Assert.Throws<InlineListException>(() => new InlineList4<int>(5));
}
