using System.Runtime.InteropServices;
using Age.Core.Collections;
using Age.Core.Extensions;

namespace Age.Tests.Core.Collections;

public class NativeRefListTest
{
    private ref struct Wrapper()
    {
        public NativeRefList<int> List;
    }

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

        AssertIt(list, [4, 5, 6], 3);

        list.Clear();

        AssertIt(list, [], 3);
    }

    [Fact]
    public void IncreaseCapacity()
    {
        var list = new NativeRefList<int>([1, 2, 3]);

        AssertIt(list, [1, 2, 3], 3);

        list.Capacity = 6;

        AssertIt(list, [1, 2, 3], 6);

        list.Add(4);
        list.Add(5);
        list.Add(6);

        AssertIt(list, [1, 2, 3, 4, 5, 6], 6);

        list.Dispose();
    }

    [Fact]
    public void DecreaseCapacity()
    {
        var list = new NativeRefList<int>(4);

        AssertIt(list, [], 4);

        list.Add(0);
        list.Add(1);
        list.Add(2);

        AssertIt(list, [0, 1, 2], 4);

        list.Capacity = 3;

        AssertIt(list, [0, 1, 2], 3);

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

    [Fact]
    public unsafe void Nested()
    {
        var wrapper = new Wrapper();

        wrapper.List.Add(1);
        wrapper.List.Add(2);
        wrapper.List.Add(3);

        AssertIt(wrapper.List, [1, 2, 3], 4);

        addMore(&wrapper);

        AssertIt(wrapper.List, [1, 2, 3, 4, 5, 6], 8);

        wrapper.List.Dispose();

        static void addMore(Wrapper* wrapper)
        {
            wrapper->List.Add(4);
            wrapper->List.Add(5);
            wrapper->List.Add(6);
        }
    }

    [Fact]
    public unsafe void NestedPointer()
    {
        var wrapper = NativeMemory.AllocZeroed<Wrapper>();

        wrapper->List.Add(1);
        wrapper->List.Add(2);
        wrapper->List.Add(3);

        AssertIt(wrapper->List, [1, 2, 3], 4);

        addMore(wrapper);

        AssertIt(wrapper->List, [1, 2, 3, 4, 5, 6], 8);

        wrapper->List.Dispose();

        static void addMore(Wrapper* wrapper)
        {
            wrapper->List.Add(4);
            wrapper->List.Add(5);
            wrapper->List.Add(6);
        }
    }
}
