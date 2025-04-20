using Age.Core;

namespace Age.Tests.Core;

public class RefStringBufferTest
{
    private static void AssertIt(RefStringBuffer buffer, string text, int capacity)
    {
        Assert.Equal(text, buffer.ToString());
        Assert.Equal(text.Length, buffer.Length);
        Assert.Equal(capacity, buffer.Capacity);
    }

    [Fact]
    public void Append()
    {
        using var buffer = new RefStringBuffer(32);

        buffer.Append("Hello");

        AssertIt(buffer, "Hello", 32);

        buffer.Append("World");

        AssertIt(buffer, "HelloWorld", 32);
    }

    [Fact]
    public void Insert()
    {
        using var buffer = new RefStringBuffer("HelloWorld");

        AssertIt(buffer, "HelloWorld", 10);

        buffer.Insert(5, [' ']);

        AssertIt(buffer, "Hello World", 20);

        buffer.Insert(11, "!!!");

        AssertIt(buffer, "Hello World!!!", 20);
    }

    [Fact]
    public void Remove()
    {
        using var buffer = new RefStringBuffer("Hello World!!!");

        buffer.Remove(11, 3);

        AssertIt(buffer, "Hello World", 14);

        buffer.Remove(5, 1);

        AssertIt(buffer, "HelloWorld", 14);
    }
}
