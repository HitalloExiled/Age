using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeRefListTest
{
    private static void AssertIt(in NativeRefList<int> list, ReadOnlySpan<int> values, int capacity)
    {
        Assert.Equal(capacity, list.Capacity);
        Assert.Equal(values.Length, list.Count);

        Assert.True(list.AsSpan().SequenceEqual(values));
    }

    private static void AssertIt(Span<int> list, ReadOnlySpan<int> values)
    {
        Assert.Equal(values.Length, list.Length);

        Assert.True(list.SequenceEqual(values));
    }

    [Fact]
    public void Add()
    {
        using NativeRefList<int> list = [1, 2, 3];

        AssertIt(list, [1, 2, 3], 3);

        list.Add(4);

        AssertIt(list, [1, 2, 3, 4], 6);
    }

    [Fact]
    public void Insert()
    {
        using NativeRefList<int> list = [1, 3, 4];

        AssertIt(list, [1, 3, 4], 3);

        list.Insert(1, 2);

        AssertIt(list, [1, 2, 3, 4], 6);

        list.Insert(4, 5);

        AssertIt(list, [1, 2, 3, 4, 5], 6);

        list.Insert(0, 0);

        AssertIt(list, [0, 1, 2, 3, 4, 5], 6);
    }

    [Fact]
    public void Index()
    {
        using NativeRefList<int> list = [1, 2, 3];

        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[^1]);
    }

    [Fact]
    public void Slice()
    {
        using NativeRefList<int> list = [1, 2, 3, 4, 5, 6, 7, 8, 9];

        var slice = list[3..6];

        AssertIt(slice, [4, 5, 6]);
    }

    [Fact]
    public void Remove()
    {
        using var list = new NativeRefList<int>([4, 5, 6]);

        AssertIt(list, [4, 5, 6], 3);

        list.Remove(5);

        AssertIt(list, [4, 6], 3);
    }

    [Fact]
    public void RemoveAt()
    {
        using var list = new NativeRefList<int>([4, 5, 6]);

        AssertIt(list, [4, 5, 6], 3);

        list.RemoveAt(1);

        AssertIt(list, [4, 6], 3);
    }

    [Fact]
    public void RemoveWithLength()
    {
        using var list = new NativeRefList<int>([1, 2, 3, 4, 5, 6]);

        AssertIt(list, [1, 2, 3, 4, 5, 6], 6);

        list.RemoveAt(2, 2);

        AssertIt(list, [1, 2, 5, 6], 6);

        list.RemoveAt(2, 2);

        AssertIt(list, [1, 2], 6);
    }

    [Fact]
    public void Clear()
    {
        using var list = new NativeRefList<int>([4, 5, 6]);

        Assert.Equal(3, list.Capacity);
        Assert.Equal(3, list.Count);

        Assert.Equal(4, list[0]);
        Assert.Equal(5, list[1]);
        Assert.Equal(6, list[2]);

        list.Clear();

        Assert.Equal(3, list.Capacity);
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void IncreaseCapacity()
    {
        var list = new NativeRefList<int>([1, 2, 3]);

        Assert.Equal(3, list.Capacity);
        Assert.Equal(3, list.Count);

        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);

        list.Capacity = 6;

        Assert.Equal(6, list.Capacity);
        Assert.Equal(3, list.Count);

        list.Add(4);
        list.Add(5);
        list.Add(6);

        Assert.Equal(6, list.Capacity);
        Assert.Equal(6, list.Count);

        list.Dispose();
    }

    [Fact]
    public void DecreaseCapacity()
    {
        var list = new NativeRefList<int>(4);

        Assert.Equal(0, list.Count);
        Assert.Equal(4, list.Capacity);

        list.Add(0);
        list.Add(1);
        list.Add(2);

        Assert.Equal(3, list.Count);
        Assert.Equal(4, list.Capacity);

        list.Capacity = 3;

        Assert.Equal(3, list.Count);
        Assert.Equal(3, list.Capacity);

        try
        {
            list.Capacity = 2;
        }
        catch (Exception exception)
        {
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        list.Dispose();
    }
}
