using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeListTest
{
    private static void AssertIt(NativeList<int> list, ReadOnlySpan<int> values, int capacity)
    {
        Assert.Equal(capacity, list.Capacity);
        Assert.Equal(values.Length, list.Count);

        var span = list.AsSpan();

        Assert.True(span.SequenceEqual(values));
    }

    private static void AssertIt(Span<int> list, ReadOnlySpan<int> values)
    {
        Assert.Equal(values.Length, list.Length);

        Assert.True(list.SequenceEqual(values));
    }

    [Fact]
    public void Create_AllocatesWithInitialElements()
    {
        using NativeList<int> list = [1, 2, 3];

        AssertIt(list, [1, 2, 3], 3);
    }

    [Fact]
    public void CreateFixed_AllocatesFixedSizeList()
    {
        using NativeList<int> list = new([1, 2, 3], true);

        AssertIt(list, [1, 2, 3], 3);
    }

    [Fact]
    public void Add_AppendsElement()
    {
        using NativeList<int> list = [1, 2, 3];

        AssertIt(list, [1, 2, 3], 3);

        list.Add(4);

        AssertIt(list, [1, 2, 3, 4], 6);
    }

    [Fact]
    public void Insert_InsertsElementAtPosition()
    {
        using NativeList<int> list = [1, 3, 4];

        AssertIt(list, [1, 3, 4], 3);

        list.Insert(1, 2);

        AssertIt(list, [1, 2, 3, 4], 6);

        list.Insert(4, 5);

        AssertIt(list, [1, 2, 3, 4, 5], 6);

        list.Insert(0, 0);

        AssertIt(list, [0, 1, 2, 3, 4, 5], 6);
    }

    [Fact]
    public void Index_AccessesElementsByIndex()
    {
        using NativeList<int> list = [1, 2, 3];

        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[^1]);
    }

    [Fact]
    public void Slice_ReturnsSubspan()
    {
        using NativeList<int> list = [1, 2, 3, 4, 5, 6, 7, 8, 9];

        var slice = list[3..6];

        AssertIt(slice, [4, 5, 6]);
    }

    [Fact]
    public void Remove_RemovesElementByValue()
    {
        using var list = new NativeList<int>([4, 5, 6]);

        AssertIt(list, [4, 5, 6], 3);

        list.Remove(5);

        AssertIt(list, [4, 6], 3);
    }

    [Fact]
    public void RemoveAt_RemovesElementByIndex()
    {
        using var list = new NativeList<int>([4, 5, 6]);

        AssertIt(list, [4, 5, 6], 3);

        list.RemoveAt(1);

        AssertIt(list, [4, 6], 3);
    }

    [Fact]
    public void RemoveWithLength_RemovesRangeOfElements()
    {
        using var list = new NativeList<int>([1, 2, 3, 4, 5, 6]);

        AssertIt(list, [1, 2, 3, 4, 5, 6], 6);

        list.RemoveAt(2, 2);

        AssertIt(list, [1, 2, 5, 6], 6);

        list.RemoveAt(2, 2);

        AssertIt(list, [1, 2], 6);
    }

    [Fact]
    public void Clear_RemovesAllElements()
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
    public void IncreaseCapacity_ExpandsCapacity()
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
    public void DecreaseCapacity_ShrinksCapacity()
    {
        using var list = new NativeList<int>(4);

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

        Assert.Throws<ArgumentOutOfRangeException>(() => list.Capacity = 2);
    }

    [Fact]
    public void DisposeShouldPass_DoesNotThrowOnDoubleDispose()
    {
        var list = new NativeList<int>([1]);

        list.Dispose();
        list.Dispose();

        Assert.True(true);
    }
}
