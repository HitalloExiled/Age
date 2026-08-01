using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.Tests.Age.Elements;

#pragma warning disable CS9113

[Collection("GPU")]
public class ElementScrollTest(GpuFixture _)
{
    [Fact]
    public void Scroll_ZeroContent_ClampsToZero()
    {
        var window = Window.CreateMock();

        var element = new FlexBox
        {
            Style =
            {
                Size     = new(100),
                Overflow = Overflow.Scroll
            },
        };

        window.UIScene!.Canvas.AppendChild(element);

        window.RenderTree.Update();

        element.Scroll = new(50, 50);

        Assert.Equal(new Point<uint>(0, 0), element.Scroll);
    }

    [Fact]
    public void Scroll_ContentLargerThanSize_ClampsToMax()
    {
        var window = Window.CreateMock();

        var element = new FlexBox
        {
            Style =
            {
                Size     = new(100),
                Overflow = Overflow.Scroll
            },
            Children =
            [
                new FlexBox
                {
                    Style =
                    {
                        Size = new(200),
                    }
                }
            ],
        };

        window.UIScene!.Canvas.AppendChild(element);

        window.RenderTree.Update();

        element.Scroll = new(150, 150);

        Assert.Equal(new Point<uint>(100, 100), element.Scroll);
    }

    [Fact]
    public void Scroll_WithinBounds_DoesNotClamp()
    {
        var window = Window.CreateMock();

        var element = new FlexBox
        {
            Style =
            {
                Size     = new(100),
                Overflow = Overflow.Scroll
            },
            Children =
            [
                new FlexBox
                {
                    Style =
                    {
                        Size = new(200),
                    }
                }
            ],
        };

        window.UIScene!.Canvas.AppendChild(element);

        window.RenderTree.Update();

        element.Scroll = new(50);

        Assert.Equal(new Point<uint>(50), element.Scroll);
    }

    [Fact]
    public void Scroll_ContentSmallerThanSize_ClampsToZero()
    {
        var window = Window.CreateMock();

        var element = new FlexBox
        {
            Style =
            {
                Size     = new(100),
                Overflow = Overflow.Scroll
            },
            Children =
            [
                new FlexBox
                {
                    Style =
                    {
                        Size = new(50),
                    }
                }
            ],
        };

        window.UIScene!.Canvas.AppendChild(element);

        window.RenderTree.Update();

        element.Scroll = new(30, 30);

        Assert.Equal(new Point<uint>(0, 0), element.Scroll);
    }

    [Fact]
    public void Scroll_IndependentAxes()
    {
        var window = Window.CreateMock();

        var element = new FlexBox
        {
            Style =
            {
                Size     = new(100),
                Overflow = Overflow.Scroll
            },
            Children =
            [
                new FlexBox
                {
                    Style =
                    {
                        Size = new(200, 50),
                    }
                }
            ],
        };

        window.UIScene!.Canvas.AppendChild(element);

        window.RenderTree.Update();

        element.Scroll = new(150, 30);

        Assert.Equal(new Point<uint>(100, 0), element.Scroll);
    }
}
