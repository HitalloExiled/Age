using Age.Core.Interop;

namespace Age.Tests.Core.Interop;

public unsafe class NativeListTests
{
    private static void AssertIt(NativeList<int> list, ReadOnlySpan<int> values, int capacity)
    {
        Assert.Equal(capacity, list.Capacity);
        Assert.Equal(values.Length, list.Count);

        Assert.True(list.AsSpan().SequenceEqual(values));
    }

    [Fact]
    public void Add()
    {
        using var list = new NativeList<int> { 1, 2, 3 };

        AssertIt(list, [1, 2, 3], 4);

        list.Add(4);

        AssertIt(list, [1, 2, 3, 4], 4);
    }

    [Fact]
    public void Remove()
    {
        using var list = new NativeList<int>([4, 5, 6]);

        AssertIt(list, [4, 5, 6], 3);

        list.Remove(1);

        AssertIt(list, [4, 6], 3);
    }

    [Fact]
    public void RemoveWithLength()
    {
        using var list = new NativeList<int>([1, 2, 3, 4, 5, 6]);

        AssertIt(list, [1, 2, 3, 4, 5, 6], 6);

        list.Remove(2, 2);

        AssertIt(list, [1, 2, 5, 6], 6);

        list.Remove(2, 2);

        AssertIt(list, [1, 2], 6);
    }

    [Fact]
    public void Clear()
    {
        using var list = new NativeList<int>([4, 5, 6]);

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
        using var list = new NativeList<int>([1, 2, 3]);

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
    }

    [Fact]
    public void DecreaseCapacity()
    {
        using var list = new NativeList<int>([1, 2, 3, 4, 5, 6]);

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
    }
}
