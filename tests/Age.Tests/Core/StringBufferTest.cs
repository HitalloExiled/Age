using Age.Core;

namespace Age.Tests.Core;

public class StringBufferTest
{
    private static void AssertIt(StringBuffer buffer, string text, int capacity)
    {
        Assert.Equal(text, buffer.ToString());
        Assert.Equal(text.Length, buffer.Length);
        Assert.Equal(capacity, buffer.Capacity);
    }

    [Fact]
    public void Append()
    {
        var buffer = new StringBuffer();

        buffer.Append("Hello");

        AssertIt(buffer, "Hello", 32);

        buffer.Append("World");

        AssertIt(buffer, "HelloWorld", 32);
    }

    [Fact]
    public void Insert()
    {
        var buffer = new StringBuffer("HelloWorld");

        AssertIt(buffer, "HelloWorld", 10);

        buffer.Insert(5, [' ']);

        AssertIt(buffer, "Hello World", 20);

        buffer.Insert(11, "!!!");

        AssertIt(buffer, "Hello World!!!", 20);
    }

    [Fact]
    public void Remove()
    {
        var buffer = new StringBuffer("Hello World!!!");

        buffer.Remove(11, 3);

        AssertIt(buffer, "Hello World", 14);

        buffer.Remove(5, 1);

        AssertIt(buffer, "HelloWorld", 14);
    }
}
