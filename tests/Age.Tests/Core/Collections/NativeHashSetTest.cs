using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public unsafe class NativeHashSetTest
{
    [Fact]
    public void Create_AllocatesWithInitialCapacity()
    {
        using var set = new NativeHashSet<int>(4);

        Assert.Equal(0, set.Count);
        Assert.True(set.Capacity > 0);
        Assert.True(set.IsCreated);
        Assert.False(set.IsDisposed);
        Assert.False(set.IsFixedSize);
    }

    [Fact]
    public void CreateFixed_SetsFixedSize()
    {
        using var set = new NativeHashSet<int>(8, true);

        Assert.True(set.IsFixedSize);
    }

    [Fact]
    public void Add_InsertsNewElements()
    {
        using var set = new NativeHashSet<int>(4);

        Assert.True(set.Add(1));
        Assert.True(set.Add(2));
        Assert.True(set.Add(3));

        Assert.Equal(3, set.Count);
        Assert.True(set.Contains(1));
        Assert.True(set.Contains(2));
        Assert.True(set.Contains(3));
    }

    [Fact]
    public void AddDuplicate_ReturnsFalseForDuplicateElement()
    {
        using var set = new NativeHashSet<int>(4);

        Assert.True(set.Add(1));
        Assert.False(set.Add(1));

        Assert.Equal(1, set.Count);
    }

    [Fact]
    public void Remove_DeletesExistingElement()
    {
        using var set = new NativeHashSet<int>(4);

        set.Add(1);
        set.Add(2);
        set.Add(3);

        Assert.True(set.Remove(2));
        Assert.Equal(2, set.Count);
        Assert.False(set.Contains(2));
        Assert.True(set.Contains(1));
        Assert.True(set.Contains(3));
    }

    [Fact]
    public void RemoveNonExisting_ReturnsFalse()
    {
        using var set = new NativeHashSet<int>(4);

        Assert.False(set.Remove(42));
    }

    [Fact]
    public void Contains_ChecksElementExistence()
    {
        using var set = new NativeHashSet<int>(4);

        Assert.False(set.Contains(1));

        set.Add(1);

        Assert.True(set.Contains(1));
        Assert.False(set.Contains(2));
    }

    [Fact]
    public void Clear_RemovesAllElements()
    {
        using var set = new NativeHashSet<int>(4);

        set.Add(1);
        set.Add(2);
        set.Add(3);

        Assert.Equal(3, set.Count);

        set.Clear();

        Assert.Equal(0, set.Count);
        Assert.False(set.Contains(1));
        Assert.False(set.Contains(2));
        Assert.False(set.Contains(3));
    }

    [Fact]
    public void ToNativeArray_ReturnsArrayWithAllElements()
    {
        using var set = new NativeHashSet<int>(8);

        set.Add(10);
        set.Add(20);
        set.Add(30);

        using var array = set.ToNativeArray();

        Assert.Equal(3, array.Length);

        var list = new List<int>(3);

        foreach (var item in array)
        {
            list.Add(item);
        }

        list.Sort();

        Assert.Equal([10, 20, 30], list);
    }

    [Fact]
    public void ToNativeArrayEmpty_ReturnsUncreatedArray()
    {
        using var set = new NativeHashSet<int>(4);

        var array = set.ToNativeArray();

        Assert.False(array.IsCreated);
    }

    [Fact]
    public void Enumerate_IteratesOverAllElements()
    {
        using var set = new NativeHashSet<int>(8);

        set.Add(1);
        set.Add(2);
        set.Add(3);
        set.Add(4);
        set.Add(5);

        var found = new bool[6];

        foreach (var item in set)
        {
            found[item] = true;
        }

        for (var i = 1; i <= 5; i++)
        {
            Assert.True(found[i]);
        }
    }

    [Fact]
    public void EnumerateEmpty_DoesNotIterate()
    {
        using var set = new NativeHashSet<int>(4);

        foreach (var _ in set)
        {
            Assert.Fail("Should not enumerate empty set");
        }
    }

    [Fact]
    public void LargeCount_HandlesManyElements()
    {
        using var set = new NativeHashSet<int>(16);

        for (var i = 0; i < 100; i++)
        {
            Assert.True(set.Add(i));
        }

        Assert.Equal(100, set.Count);

        for (var i = 0; i < 100; i++)
        {
            Assert.True(set.Contains(i));
        }
    }

    [Fact]
    public void RemoveAndReAdd_AllowsReinsertionAfterRemoval()
    {
        using var set = new NativeHashSet<int>(4);

        set.Add(1);
        set.Remove(1);
        Assert.Equal(0, set.Count);

        Assert.True(set.Add(1));
        Assert.Equal(1, set.Count);
        Assert.True(set.Contains(1));
    }

    [Fact]
    public void DisposeShouldPass_DoesNotThrowOnDoubleDispose()
    {
        var set = new NativeHashSet<int>(4);

        set.Dispose();
        set.Dispose();

        Assert.True(set.IsDisposed);
    }
}
