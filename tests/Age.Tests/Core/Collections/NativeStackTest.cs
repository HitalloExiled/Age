using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeStackTest
{
    private static void AssertIt(NativeStack<int> stack, ReadOnlySpan<int> values, int capacity)
    {
        Assert.Equal(capacity, stack.Capacity);
        Assert.Equal(values.Length, stack.Count);

        Assert.True(stack.AsSpan().SequenceEqual(values));
    }

    [Fact]
    public void Add()
    {
        using var stack = new NativeStack<int>();

        ref var first = ref stack.Push();

        first = 1;

        AssertIt(stack, [1], 4);

        stack.Push(2);

        AssertIt(stack, [1, 2], 4);
    }

    [Fact]
    public void Remove()
    {
        using var stack = new NativeStack<int>();

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        AssertIt(stack, [1, 2, 3], 4);

        stack.Pop();

        AssertIt(stack, [1, 2], 4);

        stack.Pop();

        AssertIt(stack, [1], 4);

        stack.Pop();

        AssertIt(stack, [], 4);
    }

    [Fact]
    public void Peek()
    {
        using var stack = new NativeStack<int>();

        stack.Push(1);

        AssertIt(stack, [1], 4);

        Assert.Equal(1, stack.Peek());

        stack.Peek() = 2;

        Assert.Equal(2, stack.Peek());
    }

    [Fact]
    public void Clear()
    {
        using var stack = new NativeStack<int>();

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        AssertIt(stack, [1, 2, 3], 4);

        stack.Clear();

        AssertIt(stack, [], 4);
    }

    [Fact]
    public void IncreaseCapacity()
    {
        using var stack = new NativeStack<int>(4);

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        AssertIt(stack, [1, 2, 3], 4);

        stack.Capacity = 6;

        AssertIt(stack, [1, 2, 3], 6);

        stack.Push(4);
        stack.Push(5);
        stack.Push(6);

        AssertIt(stack, [1, 2, 3, 4, 5, 6], 6);
    }

    [Fact]
    public void DecreaseCapacity()
    {
        using var stack = new NativeStack<int>(6);

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);
        stack.Push(5);
        stack.Push(6);

        AssertIt(stack, [1, 2, 3, 4, 5, 6], 6);

        stack.Capacity = 3;

        AssertIt(stack, [1, 2, 3], 3);
    }

    [Fact]
    public void Enumerate()
    {
        using var stack = new NativeStack<int>(6);

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);
        stack.Push(5);
        stack.Push(6);

        var expected = new int[] { 6, 5, 4, 3, 2, 1 };
        var actual   = new List<int>(6);

        actual.AddRange(stack);

        Assert.Equal(expected, actual);
    }
}
