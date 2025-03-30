using Age.Core.Interop;

namespace Age.Tests.Core.Interop;

public unsafe class RefListTests
{
    private static void AssertIt(scoped in RefList<int> list, ReadOnlySpan<int> values, int capacity)
    {
        Assert.Equal(capacity, list.Capacity);
        Assert.Equal(values.Length, list.Count);

        Assert.True(list.AsSpan().SequenceEqual(values));
    }

    [Fact]
    public void Add()
    {
        using var list = new RefList<int> { 1, 2, 3 };

        AssertIt(list, [1, 2, 3], 4);

        list.Add(4);

        AssertIt(list, [1, 2, 3, 4], 4);
    }

    [Fact]
    public void Insert()
    {
        using var list = new RefList<int> { 1, 3, 4 };

        AssertIt(list, [1, 3, 4], 4);

        list.Insert(1, 2);

        AssertIt(list, [1, 2, 3, 4], 4);

        list.Insert(4, 5);

        AssertIt(list, [1, 2, 3, 4, 5], 8);

        list.Insert(0, 0);

        AssertIt(list, [0, 1, 2, 3, 4, 5], 8);
    }

    [Fact]
    public void Index()
    {
        using var list = new RefList<int> { 1, 2, 3 };

        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[^1]);
    }

    [Fact]
    public void Slice()
    {
        using var list = new RefList<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        using var slice = list[3..6];

        AssertIt(slice, [4, 5, 6], 3);
    }

    [Fact]
    public void Remove()
    {
        using var list = new RefList<int>([4, 5, 6]);

        AssertIt(list, [4, 5, 6], 3);

        list.Remove(5);

        AssertIt(list, [4, 6], 3);
    }

    [Fact]
    public void RemoveAt()
    {
        using var list = new RefList<int>([4, 5, 6]);

        AssertIt(list, [4, 5, 6], 3);

        list.RemoveAt(1);

        AssertIt(list, [4, 6], 3);
    }

    [Fact]
    public void RemoveWithLength()
    {
        using var list = new RefList<int>([1, 2, 3, 4, 5, 6]);

        AssertIt(list, [1, 2, 3, 4, 5, 6], 6);

        list.RemoveAt(2, 2);

        AssertIt(list, [1, 2, 5, 6], 6);

        list.RemoveAt(2, 2);

        AssertIt(list, [1, 2], 6);
    }

    [Fact]
    public void Clear()
    {
        using var list = new RefList<int>([4, 5, 6]);

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
        var list = new RefList<int>([1, 2, 3]);

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
        var list = new RefList<int>([1, 2, 3, 4, 5, 6]);

        Assert.Equal(6, list.Capacity);
        Assert.Equal(6, list.Count);

        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
        Assert.Equal(4, list[3]);
        Assert.Equal(5, list[4]);
        Assert.Equal(6, list[5]);

        list.Capacity = 3;

        Assert.Equal(3, list.Capacity);
        Assert.Equal(3, list.Count);

        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);

        list.Dispose();
    }
}
