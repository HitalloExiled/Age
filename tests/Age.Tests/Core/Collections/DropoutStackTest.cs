using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class DropoutStackTest
{
    private static void AssertSize(DropoutStack<int> stack, int[] expected)
    {
        Assert.Equal(expected.Length, stack.Count);

        int[] actual = [..stack];

        Assert.True(expected.SequenceEqual(actual));
    }

    [Fact]
    public void Push()
    {
        var stack = new DropoutStack<int>(3);

        stack.Push(1);

        AssertSize(stack, [1]);

        stack.Push(2);

        AssertSize(stack, [1, 2]);

        stack.Push(3);

        AssertSize(stack, [1, 2, 3]);

        stack.Push(4);

        AssertSize(stack, [2, 3, 4]);

        stack.Push(5);

        AssertSize(stack, [3, 4, 5]);

        stack.Push(6);

        AssertSize(stack, [4, 5, 6]);

        stack.Push(7);

        AssertSize(stack, [5, 6, 7]);


    }

    [Fact]
    public void Pop()
    {
        var stack = new DropoutStack<int>(5);

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);
        stack.Push(5);

        AssertSize(stack, [1, 2, 3, 4, 5]);

        Assert.Equal(5, stack.Pop());

        AssertSize(stack, [1, 2, 3, 4]);

        Assert.Equal(4, stack.Pop());

        AssertSize(stack, [1, 2, 3]);

        Assert.Equal(3, stack.Pop());

        AssertSize(stack, [1, 2]);

        Assert.Equal(2, stack.Pop());

        AssertSize(stack, [1]);

        Assert.Equal(1, stack.Pop());

        AssertSize(stack, []);

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);
        stack.Push(5);
        stack.Push(6);
        stack.Push(7);

        AssertSize(stack, [3, 4, 5, 6, 7]);

        Assert.Equal(7, stack.Pop());

        AssertSize(stack, [3, 4, 5, 6]);

        Assert.Equal(6, stack.Pop());

        AssertSize(stack, [3, 4, 5]);

        Assert.Equal(5, stack.Pop());

        AssertSize(stack, [3, 4]);

        Assert.Equal(4, stack.Pop());

        AssertSize(stack, [3]);

        Assert.Equal(3, stack.Pop());

        AssertSize(stack, []);
    }
}
