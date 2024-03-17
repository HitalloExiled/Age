using Age.Numerics;

namespace Age.Tests.Numerics;

public class RectTests
{
    [Fact]
    public void Grow()
    {
        var actual = new Rect<int>();

        actual.Grow(new(10, 20,  20, 00));

        Assert.Equal(new(30, 20, 0, 0), actual);

        actual.Grow(new(05, 05, -20, 00));

        Assert.Equal(new(50, 20, -20, 0), actual);

        actual.Grow(new(20, 30, 00, 00));

        Assert.Equal(new(50, 30, -20, 0), actual);

        actual.Grow(new(45, 30, -15, 00));

        Assert.Equal(new(50, 30, -20, 0), actual);

        actual.Grow(new(10, 10, -15, -10));

        Assert.Equal(new(50, 40, -20, -10), actual);
    }
}
