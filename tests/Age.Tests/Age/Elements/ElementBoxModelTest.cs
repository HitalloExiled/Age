using Age.Elements;
using Age.Numerics;
using Age.Scenes;
using Age.Styling;

namespace Age.Tests.Age.Elements;

#pragma warning disable CS9113

[Collection("GPU")]
public class ElementBoxModelTest(GpuFixture _)
{
    private static (Window window, UIScene uiScene) SetupTree()
    {
        var window  = WindowTestExtensions.CreateTestWindow();
        var uiScene = new UIScene();

        window.AppendChild(uiScene);

        return (window, uiScene);
    }

    [Fact]
    public void WithDependents_ReturnsDefault()
    {
        var (window, uiScene) = SetupTree();

        var parent = new FlexBox();
        var child  = new TestElement { Style = { Size = new(Unit.Pc(50), Unit.Pc(50)) } };

        uiScene.Canvas.AppendChild(parent);
        parent.AppendChild(child);
        window.Connect();

        var boxModel = parent.GetBoxModel();

        Assert.Equal(default, boxModel.Content);
        Assert.Equal(default,  boxModel.Margin);
        Assert.Equal(default,  boxModel.Padding);
    }

    [Fact]
    public void AfterConnect_TriggersUpdateDisposition()
    {
        var (window, uiScene) = SetupTree();

        var parent = new FlexBox();
        var child  = new TestElement();

        uiScene.Canvas.AppendChild(parent);
        parent.AppendChild(child);
        window.Connect();

        var boxModel = parent.GetBoxModel();

        Assert.Equal(default, boxModel.Content);
    }

    [Fact]
    public void PendingLayouts_ResolvesPercentageWidth()
    {
        var (window, uiScene) = SetupTree();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            },
            Children =
            [
                new TestElement
                {
                    Style =
                    {
                        Size  = new(Unit.Pc(50), Unit.Pc(50)),
                    }
                }
            ]
        };

        uiScene.Canvas.AppendChild(parent);

        window.Connect();

        var boxModel = parent.GetBoxModel();

        Assert.Equal(new Size<uint>(100, 50), boxModel.Content);
    }

    [Fact]
    public void ByDefault_ReturnsZero()
    {
        var (window, uiScene) = SetupTree();

        var element = new TestElement();

        uiScene.Canvas.AppendChild(element);
        window.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(default, boxModel.Margin);
        Assert.Equal(default, boxModel.Padding);
        Assert.Equal(default, boxModel.Border);
        Assert.Equal(default, boxModel.Content);
        Assert.Equal(default, boxModel.Boundings.Position);
        Assert.Equal(default, boxModel.Boundings.Size);
    }

    [Fact]
    public void WithMargin_ReturnsEdges()
    {
        var (window, uiScene) = SetupTree();

        var element = new TestElement { Style = { Margin = new StyleRectEdges(Unit.Px(5), Unit.Px(10), Unit.Px(3), Unit.Px(8)) } };

        uiScene.Canvas.AppendChild(element);
        window.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(new RectEdges { Top = 5, Right = 10, Bottom = 3, Left = 8 }, boxModel.Margin);
    }

    [Fact]
    public void WithPadding_ReturnsEdges()
    {
        var (window, uiScene) = SetupTree();

        var element = new TestElement { Style = { Padding = new StyleRectEdges(Unit.Px(7), Unit.Px(4)) } };

        uiScene.Canvas.AppendChild(element);
        window.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(new RectEdges { Top = 7, Right = 4, Bottom = 7, Left = 4 }, boxModel.Padding);
    }

    [Fact]
    public void WithBorder_ReturnsEdges()
    {
        var (window, uiScene) = SetupTree();

        var element = new TestElement { Style = { Border = new Border(thickness: 3, radius: 0, color: Color.White) } };

        uiScene.Canvas.AppendChild(element);
        window.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(new RectEdges { Top = 3, Right = 3, Bottom = 3, Left = 3 }, boxModel.Border);
    }

    [Fact]
    public void Size_SetsBoundings()
    {
        var (window, uiScene) = SetupTree();

        var element = new TestElement { Style = { Size = new SizeUnit(Unit.Px(100), Unit.Px(50)) } };

        uiScene.Canvas.AppendChild(element);
        window.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(new Size<int>(100, 50), boxModel.Boundings.Size);
        Assert.Equal(0, boxModel.Boundings.Position.X);
        Assert.Equal(0, boxModel.Boundings.Position.Y);
    }

    [Fact]
    public void PaddingAndBorder_IncreaseBoundings()
    {
        var (window, uiScene) = SetupTree();

        var element = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Px(100), Unit.Px(50)),
                Padding = new StyleRectEdges(Unit.Px(5)),
                Border  = new Border(thickness: 2, radius: 0, color: Color.White),
            }
        };

        uiScene.Canvas.AppendChild(element);
        window.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(new Size<int>(114, 64), boxModel.Boundings.Size);
    }

    [Fact]
    public void BoxSizingBorder_ContentShrinks()
    {
        var (window, uiScene) = SetupTree();

        var element = new TestElement
        {
            Style =
            {
                Size      = new SizeUnit(Unit.Px(96), Unit.Px(46)),
                Padding   = new StyleRectEdges(Unit.Px(5)),
                Border    = new Border(thickness: 2, radius: 0, color: Color.White),
                BoxSizing = BoxSizing.Border,
            }
        };

        uiScene.Canvas.AppendChild(element);
        window.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(new Size<int>(110, 60), boxModel.Boundings.Size);
        Assert.Equal(default, boxModel.Content);
    }

    [Fact]
    public void ContentAccumulatesFromChildren()
    {
        var (window, uiScene) = SetupTree();

        var parent = new FlexBox();
        var child1 = new TestElement { Style = { Size = new SizeUnit(Unit.Px(30), Unit.Px(20)) } };
        var child2 = new TestElement { Style = { Size = new SizeUnit(Unit.Px(50), Unit.Px(40)) } };

        uiScene.Canvas.AppendChild(parent);
        parent.AppendChild(child1);
        parent.AppendChild(child2);
        window.Connect();

        var boxModel = parent.GetBoxModel();

        Assert.Equal(new Size<uint>(80, 40), boxModel.Content);
    }

    [Fact]
    public void WithAllProperties_ReturnsExpectedValues()
    {
        var (window, uiScene) = SetupTree();

        var element = new TestElement
        {
            Style =
            {
                Margin  = new StyleRectEdges(Unit.Px(3), Unit.Px(6), Unit.Px(9), Unit.Px(12)),
                Padding = new StyleRectEdges(Unit.Px(8), Unit.Px(4)),
                Border  = new Border(
                    new BorderSide { Thickness = 2, Color = Color.White },
                    new BorderSide { Thickness = 3, Color = Color.White }
                ),
                Size = new SizeUnit(Unit.Px(200), Unit.Px(100)),
            }
        };

        uiScene.Canvas.AppendChild(element);
        window.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(new RectEdges { Top = 3, Right = 6, Bottom = 9, Left = 12 }, boxModel.Margin);
        Assert.Equal(new RectEdges { Top = 8, Right = 4, Bottom = 8, Left = 4 }, boxModel.Padding);
        Assert.Equal(new RectEdges { Top = 3, Right = 2, Bottom = 3, Left = 2 }, boxModel.Border);
        Assert.Equal(default, boxModel.Content);
        Assert.Equal(new Size<int>(212, 122), boxModel.Boundings.Size);
    }
}
