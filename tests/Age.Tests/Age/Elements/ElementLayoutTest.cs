using Age.Elements;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Scenes;
using Age.Styling;
using Age.Tests.Age.Fixtures;
using Image = Age.Styling.Image;

namespace Age.Tests.Age.Elements;

#pragma warning disable CS9113

[Collection("GPU")]
public class ElementLayoutTest(GpuFixture _)
{
    [Fact]
    public void PercentageSize_WithPixelMinAndMax_ClampsToMin()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
                MinSize = new SizeUnit(Unit.Px(150), Unit.Px(150)),
                MaxSize = new SizeUnit(Unit.Px(200), Unit.Px(200)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(150, 150), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void PercentageSize_WithPixelMinAndMax_ClampsToMax()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
                MinSize = new SizeUnit(Unit.Px(10), Unit.Px(10)),
                MaxSize = new SizeUnit(Unit.Px(40), Unit.Px(40)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(40, 40), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void PercentageSize_WithPixelMinOnly_ClampsToMin()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
                MinSize = new SizeUnit(Unit.Px(150), Unit.Px(150)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(150, 150), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void PercentageSize_WithPixelMaxOnly_ClampsToMax()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
                MaxSize = new SizeUnit(Unit.Px(40), Unit.Px(40)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(40, 40), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void PixelSize_WithPercentageMinAndMax_NoClamp()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Px(100), Unit.Px(100)),
                MinSize = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
                MaxSize = new SizeUnit(Unit.Pc(90), Unit.Pc(90)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(100, 90), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void PixelSize_WithPercentageMinAndMax_ClampsToMin()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Px(50), Unit.Px(50)),
                MinSize = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
                MaxSize = new SizeUnit(Unit.Pc(90), Unit.Pc(90)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(100, 50), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void PixelSize_WithPercentageMinOnly_ClampsToMin()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Px(50), Unit.Px(50)),
                MinSize = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(100, 50), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void PixelSize_WithPercentageMaxOnly_ClampsToMax()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size    = new SizeUnit(Unit.Px(200), Unit.Px(200)),
                MaxSize = new SizeUnit(Unit.Pc(90), Unit.Pc(90)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(180, 90), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void EmSize_ResolvesAgainstFontSize()
    {
        var window = Window.CreateMock();

        var element = new TestElement { Style = { Size = new SizeUnit(Unit.Em(2), Unit.Em(3)) } };

        window.Scene!.Canvas!.AppendChild(element);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(32, 48), element.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void MinSizePixel_Resolves()
    {
        var window = Window.CreateMock();

        var element = new TestElement { Style = { MinSize = new SizeUnit(Unit.Px(50), Unit.Px(50)) } };

        window.Scene!.Canvas!.AppendChild(element);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(50, 50), element.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void MinSizeEm_Resolves()
    {
        var window = Window.CreateMock();

        var element = new TestElement { Style = { MinSize = new SizeUnit(Unit.Em(3), Unit.Em(3)) } };

        window.Scene!.Canvas!.AppendChild(element);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(48, 48), element.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void MaxSizePixel_ClampsContent()
    {
        var window = Window.CreateMock();

        var element = new TestElement { Style = { MaxSize = new SizeUnit(Unit.Px(100), Unit.Px(100)) } };
        var child   = new TestElement { Style = { Size = new SizeUnit(Unit.Px(200), Unit.Px(200)) } };

        window.Scene!.Canvas!.AppendChild(element);
        element.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(100, 100), element.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void MaxSizeEm_ClampsContent()
    {
        var window = Window.CreateMock();

        var element = new TestElement { Style = { MaxSize = new SizeUnit(Unit.Em(5), Unit.Em(5)) } };
        var child   = new TestElement { Style = { Size = new SizeUnit(Unit.Px(200), Unit.Px(200)) } };

        window.Scene!.Canvas!.AppendChild(element);
        element.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(80, 80), element.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void VerticalStack_AccumulatesContent()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size           = new(200, 200),
                StackDirection = StackDirection.Vertical,
            }
        };
        var child1 = new TestElement { Style = { Size = new SizeUnit(Unit.Px(30), Unit.Px(20)) } };
        var child2 = new TestElement { Style = { Size = new SizeUnit(Unit.Px(50), Unit.Px(40)) } };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child1);
        parent.AppendChild(child2);
        window.RenderTree.Update();

        Assert.Equal(new Size<uint>(50, 60), parent.GetBoxModel().Content);
    }

    [Fact]
    public void InvisibleChild_NotCounted()
    {
        var window = Window.CreateMock();

        var parent  = new FlexBox();
        var visible = new TestElement { Style = { Size = new SizeUnit(Unit.Px(30), Unit.Px(20)) } };
        var hidden  = new TestElement { Style = { Size = new SizeUnit(Unit.Px(80), Unit.Px(70)) } };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(visible);
        parent.AppendChild(hidden);
        hidden.Visible = false;
        window.RenderTree.Update();

        Assert.Equal(new Size<uint>(30, 20), parent.GetBoxModel().Content);
    }

    [Fact]
    public void NonLayoutableChild_Ignored()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox();
        var child  = new TestElement { Style = { Size = new SizeUnit(Unit.Px(30), Unit.Px(20)) } };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        parent.AppendChild(new Empty());
        window.RenderTree.Update();

        Assert.Equal(new Size<uint>(30, 20), parent.GetBoxModel().Content);
    }

    [Fact]
    public void LayoutableChild_NotElement_UsesBoundings()
    {
        var window = Window.CreateMock();

        var parent     = new FlexBox();
        var layoutable = new TestLayoutable();
        var child      = new TestElement { Style = { Size = new SizeUnit(Unit.Px(30), Unit.Px(20)) } };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(layoutable);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<uint>(30, 20), parent.GetBoxModel().Content);
    }

    [Fact]
    public void VisibilityChanged_HiddenToVisible_AddsDependent()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        child.Visible = false;
        child.Visible = true;
        window.RenderTree.Update();

        Assert.Equal(new Size<uint>(100, 50), parent.GetBoxModel().Content);
    }

    [Theory]
    [InlineData(StackDirection.Horizontal, ContentJustification.End, 100, 150)]
    [InlineData(StackDirection.Horizontal, ContentJustification.Center, 50, 100)]
    [InlineData(StackDirection.Horizontal, ContentJustification.SpaceAround, 25, 125)]
    [InlineData(StackDirection.Horizontal, ContentJustification.SpaceBetween, 0, 150)]
    [InlineData(StackDirection.Horizontal, ContentJustification.SpaceEvenly, 33, 116)]
    [InlineData(StackDirection.Vertical, ContentJustification.End, 120, 160)]
    [InlineData(StackDirection.Vertical, ContentJustification.Center, 60, 100)]
    [InlineData(StackDirection.Vertical, ContentJustification.SpaceAround, 30, 130)]
    [InlineData(StackDirection.Vertical, ContentJustification.SpaceBetween, 0, 160)]
    [InlineData(StackDirection.Vertical, ContentJustification.SpaceEvenly, 40, 120)]
    public void ContentJustification_OffsetsChildren(StackDirection direction, ContentJustification justification, int expectedFirst, int expectedSecond)
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size                = direction == StackDirection.Horizontal ? new(200, 100) : new(100, 200),
                StackDirection      = direction,
                ContentJustification = justification,
            }
        };
        var first  = new TestElement { Style = { Size = new SizeUnit(Unit.Px(50), Unit.Px(40)) } };
        var second = new TestElement { Style = { Size = new SizeUnit(Unit.Px(50), Unit.Px(40)) } };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(first);
        parent.AppendChild(second);
        window.RenderTree.Update();

        var firstPosition  = first.GetBoxModel().Boundings.Position;
        var secondPosition = second.GetBoxModel().Boundings.Position;

        if (direction == StackDirection.Horizontal)
        {
            Assert.Equal(expectedFirst, firstPosition.X);
            Assert.Equal(expectedSecond, secondPosition.X);
        }
        else
        {
            Assert.Equal(expectedFirst, firstPosition.Y);
            Assert.Equal(expectedSecond, secondPosition.Y);
        }
    }

    [Fact]
    public void Alignment_Center_OffsetsBothAxes()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size      = new SizeUnit(Unit.Px(50), Unit.Px(40)),
                Alignment = Alignment.Center,
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Point<int>(75, 30), child.GetBoxModel().Boundings.Position);
    }

    [Fact]
    public void ItemsAlignment_Center_OffsetsVerticalAxis()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size           = new(200, 100),
                ItemsAlignment = ItemsAlignment.Center,
            }
        };
        var child = new TestElement { Style = { Size = new SizeUnit(Unit.Px(50), Unit.Px(40)) } };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Point<int>(0, 30), child.GetBoxModel().Boundings.Position);
    }

    [Fact]
    public void VerticalStack_NoJustification_ChildCenterAligns()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size           = new(200, 300),
                StackDirection = StackDirection.Vertical,
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size      = new SizeUnit(Unit.Px(100), Unit.Px(100)),
                Alignment = Alignment.Center,
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Point<int>(50, 100), child.GetBoxModel().Boundings.Position);
    }

    [Fact]
    public void Alignment_Bottom_OffsetsToBottom()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size      = new SizeUnit(Unit.Px(50), Unit.Px(40)),
                Alignment = Alignment.Bottom,
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(60, child.GetBoxModel().Boundings.Position.Y);
    }

    [Fact]
    public void OnStyleChanged_BackgroundColor_UpdatesBoxColor()
    {
        var window = Window.CreateMock();

        var child = new TestElement
        {
            Style =
            {
                BackgroundColor = Color.Red,
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(Color.Red, child.ComputedStyle.BackgroundColor);
    }

    [Fact]
    public void OnStyleChanged_BackgroundImage_MissingUri_DisposesCommand()
    {
        var window = Window.CreateMock();

        var child = new TestElement
        {
            Style =
            {
                BackgroundImage = new Image("nonexistent.png"),
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        Assert.NotNull(child.ComputedStyle.BackgroundImage);
    }

    [Fact]
    public void ResolveImageSize_Fit_UsesElementBoundings()
    {
        var window = Window.CreateMock();

        using var texture = new Texture2D(new Texture2D.CreateInfo { Size = new(1) });

        var child = new TestElement
        {
            Style =
            {
                Size            = new(100, 50),
                BackgroundImage = new Image(texture)
                {
                    Size     = ImageSize.Fit(),
                    Position = new(Unit.Px(10), Unit.Px(20)),
                },
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(100, 50), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void ResolveImageSize_KeepAspect_ScaledToSmallestDimension()
    {
        var window = Window.CreateMock();

        using var texture = new Texture2D(new Texture2D.CreateInfo { Size = new(2, 1) });

        var child = new TestElement
        {
            Style =
            {
                Size            = new(100, 50),
                BackgroundImage = new Image(texture)
                {
                    Size     = ImageSize.KeepAspect(),
                    Position = new(Unit.Px(5), Unit.Px(3)),
                },
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        child.Style.Size = new SizeUnit(0, 0);
        window.RenderTree.Update();

        Assert.Equal(default, child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void ResolveImageSize_Size_NoRepeat_ResolvesExplicitSize()
    {
        var window = Window.CreateMock();

        using var texture = new Texture2D(new Texture2D.CreateInfo { Size = new(1) });

        var child = new TestElement
        {
            Style =
            {
                Size            = new(100, 50),
                BackgroundImage = new Image(texture)
                {
                    Size   = ImageSize.Size(Unit.Px(20), Unit.Px(30)),
                    Repeat = ImageRepeat.NoRepeat,
                },
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        child.Style.Size = new SizeUnit(0, 0);
        window.RenderTree.Update();

        Assert.Equal(default, child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void ResolveImageSize_Size_Repeat_RepeatsAcrossBoundings()
    {
        var window = Window.CreateMock();

        using var texture = new Texture2D(new Texture2D.CreateInfo { Size = new(1) });

        var child = new TestElement
        {
            Style =
            {
                Size            = new(100, 50),
                BackgroundImage = new Image(texture)
                {
                    Size   = ImageSize.Size(Unit.Px(20), Unit.Px(30)),
                    Repeat = ImageRepeat.Repeat,
                },
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(100, 50), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void OnStyleChanged_BorderClearedToNull_ResetsBorder()
    {
        var window = Window.CreateMock();

        var child = new TestElement
        {
            Style =
            {
                Border = new Border(1, 0, Color.Red),
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        Assert.NotNull(child.ComputedStyle.Border);

        child.Style.Border = null;

        Assert.Null(child.ComputedStyle.Border);
    }

    [Fact]
    public void OnStyleChanged_AllNullSize_MakesContentDependent()
    {
        var window = Window.CreateMock();

        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(),
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new SizeUnit(), child.ComputedStyle.Size);
    }

    [Fact]
    public void OnStyleChanged_RelativeMarginAndPadding_AddsParentDependencies()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Margin  = new StyleRectEdges(Unit.Pc(10)),
                Padding = new StyleRectEdges(Unit.Pc(10)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.NotNull(child.ComputedStyle.Margin);
        Assert.NotNull(child.ComputedStyle.Padding);
    }

    [Fact]
    public void OnStyleChanged_RemovingRelativeSize_RemovesFromDependents()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        child.Style.Size = new SizeUnit(Unit.Px(50), Unit.Px(50));

        Assert.Equal(Unit.Px(50), child.ComputedStyle.Size!.Value.Width);
        Assert.Equal(Unit.Px(50), child.ComputedStyle.Size!.Value.Height);
    }

    [Fact]
    public void OnStyleChanged_OverflowClipping_ReleasesScrollBarAndCreatesStencil()
    {
        var window = Window.CreateMock();

        var child = new TestElement
        {
            Style =
            {
                Size     = new SizeUnit(Unit.Px(50), Unit.Px(50)),
                Overflow = Overflow.Clipping,
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(Overflow.Clipping, child.ComputedStyle.Overflow);

        child.Style.Overflow = Overflow.None;

        Assert.Equal(Overflow.None, child.ComputedStyle.Overflow);
    }

    [Fact]
    public void DetachChild_WithRelativeVisibleElement_RemovesFromDependents()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        parent.DetachChild(child);

        Assert.Null(child.Parent);
    }

    [Fact]
    public void DetachChild_WithIndependentVisibleElement_DoesNotTouchDependents()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(Unit.Px(50), Unit.Px(50)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        parent.DetachChild(child);

        Assert.Null(child.Parent);
    }

    [Fact]
    public void DetachChild_WithInvisibleElement_SkipsRenderableBookkeeping()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        child.Visible = false;

        parent.DetachChild(child);

        Assert.Null(child.Parent);
    }

    [Fact]
    public void DetachChild_WithNonElementLayoutable_RemovesLayoutable()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var layoutable = new TestLayoutable();

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(layoutable);
        window.RenderTree.Update();

        parent.DetachChild(layoutable);

        Assert.Null(layoutable.Parent);
    }

    [Fact]
    public void VerticalStack_ChildWithinSpace_ReservesAvailableSpace()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size           = new(200, 100),
                StackDirection = StackDirection.Vertical,
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(Unit.Pc(50), Unit.Pc(50)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(200, 100), parent.GetBoxModel().Boundings.Size);
        Assert.Equal(new Size<int>(100, 50), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void VerticalStack_ChildExceedingSpace_ClampsToAvailable()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size           = new(200, 100),
                StackDirection = StackDirection.Vertical,
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(Unit.Pc(150), Unit.Pc(150)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(300, 100), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void HorizontalStack_ChildExceedingSpace_ClampsToAvailable()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Size = new SizeUnit(Unit.Pc(150), Unit.Pc(50)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(200, 50), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void EmMarginAndPadding_ResolvesToFontSize()
    {
        var window = Window.CreateMock();

        var child = new TestElement
        {
            Style =
            {
                Margin  = new StyleRectEdges(Unit.Em(2)),
                Padding = new StyleRectEdges(Unit.Em(1)),
            }
        };

        window.Scene!.Canvas!.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(32, 32), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void AutoSizeParent_WithPercentageMarginChild_GrowsContent()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox();
        var child = new TestElement
        {
            Style =
            {
                Size   = new(50, 50),
                Margin = new StyleRectEdges(Unit.Pc(10)),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(new Size<int>(60, 60), parent.GetBoxModel().Boundings.Size);
        Assert.Equal(new Size<int>(50, 50), child.GetBoxModel().Boundings.Size);
    }

    [Fact]
    public void InlineChildBaseline_NonElementLayoutable_RaisesParentBaseline()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestLayoutable
        {
            Style = new Style
            {
                Baseline = Unit.Px(20),
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        Assert.Equal(20, parent.BaseLine);
    }

    [Fact]
    public void InlineChildBaseline_ElementWithoutAlignment_RaisesParentBaseline()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement();

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        parent.Style.Size = new(220, 100);
        window.RenderTree.Update();

        Assert.Equal(20, parent.BaseLine);
    }

    [Fact]
    public void InlineChildBaseline_ElementCenterAlignment_KeepsParentBaseline()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Alignment = Alignment.Center,
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        parent.Style.Size = new(220, 100);
        window.RenderTree.Update();

        Assert.Equal(-1, parent.BaseLine);
    }

    [Fact]
    public void InlineChildBaseline_ElementTopAlignment_KeepsParentBaseline()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Alignment = Alignment.Top,
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        parent.Style.Size = new(220, 100);
        window.RenderTree.Update();

        Assert.Equal(-1, parent.BaseLine);
    }

    [Fact]
    public void InlineChildBaseline_ElementStartInVerticalStack_KeepsParentBaseline()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size           = new(200, 100),
                StackDirection = StackDirection.Vertical,
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Alignment = Alignment.Start,
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        parent.Style.Size = new(200, 120);
        window.RenderTree.Update();

        Assert.Equal(-1, parent.BaseLine);
    }

    [Fact]
    public void InlineChildBaseline_LowerSecondChild_KeepsHighest()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var first = new TestLayoutable();
        var second = new TestLayoutable();

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(first);
        parent.AppendChild(second);
        window.RenderTree.Update();

        Assert.Equal(30, parent.BaseLine);
    }

    [Fact]
    public void InlineChildBaseline_ElementLeftInHorizontalStack_RaisesParentBaseline()
    {
        var window = Window.CreateMock();

        var parent = new FlexBox
        {
            Style =
            {
                Size = new(200, 100),
            }
        };
        var child = new TestElement
        {
            Style =
            {
                Alignment = Alignment.Left,
            }
        };

        window.Scene!.Canvas!.AppendChild(parent);
        parent.AppendChild(child);
        window.RenderTree.Update();

        parent.Style.Size = new(220, 100);
        window.RenderTree.Update();

        Assert.Equal(20, parent.BaseLine);
    }

    private sealed class TestLayoutable : Layoutable
    {
        public override string NodeName => nameof(TestLayoutable);

        public Style? Style { get; set; }

        internal override bool IsParentDependent => false;

        internal override void UpdateLayout()
        {
            if (this.Style?.Baseline is { } baseline)
            {
                this.BaseLine = (int)float.Round(Unit.Resolve(baseline, 0, 0));
            }
        }
    }
}
