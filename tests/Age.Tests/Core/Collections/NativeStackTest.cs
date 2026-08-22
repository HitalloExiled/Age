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
    public void Create_AllocatesWithInitialElements()
    {
        using NativeStack<int> stack = [1, 2, 3, 4];

        AssertIt(stack, [1, 2, 3, 4], 4);
    }

    [Fact]
    public void Push_AddsElementsToTop()
    {
        using var stack = new NativeStack<int>();

        stack.Push(1);

        AssertIt(stack, [1], 4);

        stack.Push(2);

        AssertIt(stack, [1, 2], 4);
    }

    [Fact]
    public void Pop_RemovesTopElement()
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
    public void Peek_ReturnsTopElementWithoutRemoving()
    {
        using var stack = new NativeStack<int>();

        stack.Push(1);

        AssertIt(stack, [1], 4);

        Assert.Equal(1, stack.Peek());
    }

    [Fact]
    public void Clear_RemovesAllElements()
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
    public void IncreaseCapacity_ExpandsCapacity()
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
    public void DecreaseCapacity_ShrinksCapacity()
    {
        using var stack = new NativeStack<int>(4);

        Assert.Equal(0, stack.Count);
        Assert.Equal(4, stack.Capacity);

        stack.Push(0);
        stack.Push(1);
        stack.Push(2);

        Assert.Equal(3, stack.Count);
        Assert.Equal(4, stack.Capacity);

        stack.Capacity = 3;

        Assert.Equal(3, stack.Count);
        Assert.Equal(3, stack.Capacity);

        Assert.Throws<ArgumentOutOfRangeException>(() => stack.Capacity = 2);
    }

    [Fact]
    public void Enumerate_IteratesInLifoOrder()
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

        foreach (var item in stack)
        {
            actual.AddRange(item);
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_ReturnsArrayInLifoOrder()
    {
        using var stack = new NativeStack<int>(6);

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);
        stack.Push(5);
        stack.Push(6);

        var expected = new int[] { 6, 5, 4, 3, 2, 1 };

        Assert.Equal(expected, stack.ToArray());
    }

    [Fact]
    public void DisposeShouldPass_DoesNotThrowOnDoubleDispose()
    {
        var stack = new NativeStack<int>([1]);

        stack.Dispose();
        stack.Dispose();

        Assert.True(true);
    }
}
