using Age.Core.Interop;

namespace Age.Tests.Core.Interop;

public unsafe class NativeListTests
{
    [Fact]
    public void Add()
    {
        using var list = new NativeList<int> { 1, 2, 3 };

        Assert.Equal(4, list.Capacity);
        Assert.Equal(3, list.Count);

        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Remove()
    {
        using var list = new NativeList<int>([4, 5, 6]);

        Assert.Equal(3, list.Capacity);
        Assert.Equal(3, list.Count);

        Assert.Equal(4, list[0]);
        Assert.Equal(5, list[1]);
        Assert.Equal(6, list[2]);

        list.Remove(1);

        Assert.Equal(3, list.Capacity);
        Assert.Equal(2, list.Count);

        Assert.Equal(4, list[0]);
        Assert.Equal(6, list[1]);
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
