using Age.Numerics;

namespace Age.Tests.Age.Elements;

public class ElementScrollTest
{
    [Fact]
    public void Scroll_ZeroContent_ClampsToZero()
    {
        var element = new TestElement();
        ref var content = ref ElementAccessor.GetContent(element);
        ref var size    = ref ElementAccessor.GetSize(element);

        content = new(0, 0);
        size    = new(100, 100);

        element.Scroll = new(50, 50);

        Assert.Equal(new Point<uint>(0, 0), element.Scroll);
    }

    [Fact]
    public void Scroll_ContentLargerThanSize_ClampsToMax()
    {
        var element = new TestElement();
        ref var content = ref ElementAccessor.GetContent(element);
        ref var size    = ref ElementAccessor.GetSize(element);

        content = new(200, 200);
        size    = new(100, 100);

        element.Scroll = new(150, 150);

        Assert.Equal(new Point<uint>(100, 100), element.Scroll);
    }

    [Fact]
    public void Scroll_WithinBounds_DoesNotClamp()
    {
        var element = new TestElement();
        ref var content = ref ElementAccessor.GetContent(element);
        ref var size    = ref ElementAccessor.GetSize(element);

        content = new(200, 200);
        size    = new(100, 100);

        element.Scroll = new(50, 50);

        Assert.Equal(new Point<uint>(50, 50), element.Scroll);
    }

    [Fact]
    public void Scroll_ContentSmallerThanSize_ClampsToZero()
    {
        var element = new TestElement();
        ref var content = ref ElementAccessor.GetContent(element);
        ref var size    = ref ElementAccessor.GetSize(element);

        content = new(50, 50);
        size    = new(100, 100);

        element.Scroll = new(30, 30);

        Assert.Equal(new Point<uint>(0, 0), element.Scroll);
    }

    [Fact]
    public void Scroll_IndependentAxes()
    {
        var element = new TestElement();
        ref var content = ref ElementAccessor.GetContent(element);
        ref var size    = ref ElementAccessor.GetSize(element);

        content = new(200, 50);
        size    = new(100, 100);

        element.Scroll = new(150, 30);

        Assert.Equal(new Point<uint>(100, 0), element.Scroll);
    }
}
