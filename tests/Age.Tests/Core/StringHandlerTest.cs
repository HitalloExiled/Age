using Age.Core;

namespace Age.Tests.Core;

public class StringHandlerTest
{
    private static void AssertIt(StringHandler handler, string text, int capacity)
    {
        Assert.Equal(text, handler.ToString());
        Assert.Equal(text.Length, handler.Length);
        Assert.Equal(capacity, handler.Capacity);
    }

    [Fact]
    public void Append()
    {
        var handler = new StringHandler();

        handler.Append("Hello");

        AssertIt(handler, "Hello", 32);

        handler.Append("World");

        AssertIt(handler, "HelloWorld", 32);
    }

    [Fact]
    public void Insert()
    {
        var handler = new StringHandler("HelloWorld");

        AssertIt(handler, "HelloWorld", 10);

        handler.Insert([' '], 5);

        AssertIt(handler, "Hello World", 20);

        handler.Insert("!!!", 11);

        AssertIt(handler, "Hello World!!!", 20);
    }

    [Fact]
    public void Remove()
    {
        var handler = new StringHandler("Hello World!!!");

        handler.Remove(11, 3);

        AssertIt(handler, "Hello World", 14);

        handler.Remove(5, 1);

        AssertIt(handler, "HelloWorld", 14);
    }
}
