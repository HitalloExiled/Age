using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.Tests.Age.Elements;

#pragma warning disable CS9113

[Collection("GPU")]
public class ElementBoxModelTest(GpuFixture _)
{
    [Fact]
    public void BoxModel_PendingLayouts_WithDependents()
    {
        var parent = new FlexBox();
        var child  = new TestElement();

        parent.AppendChild(child);
        parent.Connect();

        ElementAccessor.GetDependents(parent).Add(child);

        ElementAccessor.MakeDirty(parent);

        var boxModel = parent.GetBoxModel();

        Assert.Equal(default, boxModel.Content);
        Assert.Equal(default,  boxModel.Margin);
        Assert.Equal(default,  boxModel.Padding);
    }

    [Fact]
    public void BoxModel_PendingLayouts_TriggersUpdateDisposition()
    {
        var parent = new FlexBox();
        var child  = new TestElement();

        parent.AppendChild(child);
        parent.Connect();

        ElementAccessor.GetDependents(parent).Add(child);
        ElementAccessor.GetChildsChanged(child) = true;

        ElementAccessor.MakeDirty(parent);

        var boxModel = parent.GetBoxModel();

        Assert.Equal(default, boxModel.Content);
    }

    [Fact]
    public void BoxModel_PendingLayouts_ResolvesPercentageWidth()
    {
        var parent = new FlexBox();
        var child  = new TestElement();

        parent.AppendChild(child);
        parent.Connect();

        ElementAccessor.GetSize(parent)      = new(200, 100);
        ElementAccessor.GetBoundings(parent) = new(200, 100);

        ElementAccessor.GetSize(child) = new(50, 30);

        child.ComputedStyle.Size = new(Unit.Pc(50), Unit.Pc(50));

        ElementAccessor.GetContentDependencies(parent) = default; // Dependency.None
        ElementAccessor.GetParentDependencies(child)   = Element.Dependency.Width; // Dependency.Width

        ElementAccessor.GetDependents(parent).Add(child);
        ElementAccessor.GetChildsChanged(parent) = true;

        ElementAccessor.MakeDirty(parent);

        var boxModel = parent.GetBoxModel();

        Assert.False(boxModel.Equals(default));
    }
    [Fact]
    public void BoxModel_Default_AllZero()
    {
        var element = new TestElement();

        element.Connect();

        var boxModel = element.GetBoxModel();

        Assert.Equal(default, boxModel.Margin);
        Assert.Equal(default, boxModel.Padding);
        Assert.Equal(default, boxModel.Border);
        Assert.Equal(default, boxModel.Content);
        Assert.Equal(default, boxModel.Boundings.Position);
        Assert.Equal(default, boxModel.Boundings.Size);
    }

    [Fact]
    public void BoxModel_Margin_SetsCorrectly()
    {
        var element = new TestElement();

        element.Connect();

        element.ComputedStyle.Margin = new StyleRectEdges(Unit.Px(5), Unit.Px(10), Unit.Px(3), Unit.Px(8));
        ElementAccessor.MakeDirty(element);

        var boxModel = element.GetBoxModel();

        Assert.Equal(new RectEdges { Top = 5, Right = 10, Bottom = 3, Left = 8 }, boxModel.Margin);
    }

    [Fact]
    public void BoxModel_Padding_SetsCorrectly()
    {
        var element = new TestElement();

        element.Connect();

        element.ComputedStyle.Padding = new StyleRectEdges(Unit.Px(4), Unit.Px(7));
        ElementAccessor.MakeDirty(element);

        var boxModel = element.GetBoxModel();

        Assert.Equal(new RectEdges { Top = 7, Right = 4, Bottom = 7, Left = 4 }, boxModel.Padding);
    }

    [Fact]
    public void BoxModel_Border_SetsCorrectly()
    {
        var element = new TestElement();

        element.Connect();

        ElementAccessor.GetBorder(element) = new() { Top = 3, Right = 3, Bottom = 3, Left = 3 };
        ElementAccessor.MakeDirty(element);

        var boxModel = element.GetBoxModel();

        Assert.Equal(new RectEdges { Top = 3, Right = 3, Bottom = 3, Left = 3 }, boxModel.Border);
    }

    [Fact]
    public void BoxModel_Size_SetsBoundings()
    {
        var element = new TestElement();

        element.Connect();

        ElementAccessor.GetSize(element)    = new(100, 50);
        ElementAccessor.GetBoundings(element) = new(100, 50);

        var boxModel = element.GetBoxModel();

        Assert.Equal(new Size<int>(100, 50), boxModel.Boundings.Size);
        Assert.Equal(0, boxModel.Boundings.Position.X);
        Assert.Equal(0, boxModel.Boundings.Position.Y);
    }

    [Fact]
    public void BoxModel_PaddingAndBorder_IncreaseBoundings()
    {
        var element = new TestElement();

        element.Connect();

        ElementAccessor.GetSize(element)      = new(100, 50);
        ElementAccessor.GetPadding(element)   = new() { Top = 5, Right = 5, Bottom = 5, Left = 5 };
        ElementAccessor.GetBorder(element)    = new() { Top = 2, Right = 2, Bottom = 2, Left = 2 };
        ElementAccessor.GetBoundings(element) = new(114, 64);

        var boxModel = element.GetBoxModel();

        Assert.Equal(new Size<int>(114, 64), boxModel.Boundings.Size);
    }

    [Fact]
    public void BoxModel_BoxSizingBorder_ContentShrinks()
    {
        var element = new TestElement();

        element.Connect();

        ElementAccessor.GetSize(element)      = new(96, 46);
        ElementAccessor.GetPadding(element)   = new() { Top = 5, Right = 5, Bottom = 5, Left = 5 };
        ElementAccessor.GetBorder(element)    = new() { Top = 2, Right = 2, Bottom = 2, Left = 2 };
        ElementAccessor.GetBoundings(element) = new(110, 60);

        var boxModel = element.GetBoxModel();

        // box-sizing: border subtracts only border from resolved size
        // size = (100-4, 50-4) = (96, 46)
        // Boundings = size(96,46) + padding(10,10) + border(4,4) = (110, 60)
        Assert.Equal(new Size<int>(110, 60), boxModel.Boundings.Size);
        Assert.Equal(default, boxModel.Content);
    }

    [Fact]
    public void BoxModel_ContentAccumulatesFromChildren()
    {
        var parent = new FlexBox();
        var child1 = new TestElement();
        var child2 = new TestElement();

        parent.AppendChild(child1);
        parent.AppendChild(child2);
        parent.Connect();

        // Set children's sizes directly (no dirty → CalculateLayout won't overwrite)
        ElementAccessor.GetSize(child1) = new(30, 20);
        ElementAccessor.GetSize(child2) = new(50, 40);

        // Mark parent dirty so its CalculateLayout accumulates children
        ElementAccessor.MakeDirty(parent);

        var boxModel = parent.GetBoxModel();

        Assert.Equal(new Size<uint>(80, 40), boxModel.Content);
    }

    [Fact]
    public void BoxModel_MarginPaddingBorderCombined_AllFieldsMatch()
    {
        var element = new TestElement();

        element.Connect();

        ElementAccessor.GetMargin(element)   = new() { Top = 3, Right = 6, Bottom = 9, Left = 12 };
        ElementAccessor.GetPadding(element)  = new() { Top = 8, Right = 4, Bottom = 8, Left = 4 };
        ElementAccessor.GetBorder(element)   = new() { Top = 3, Right = 2, Bottom = 3, Left = 2 };
        ElementAccessor.GetSize(element)     = new(200, 100);
        ElementAccessor.GetBoundings(element) = new(212, 122);

        var boxModel = element.GetBoxModel();

        // margin: top=3, right=6, bottom=9, left=12 (from StyleRectEdges TRBL)
        Assert.Equal(new RectEdges { Top = 3, Right = 6, Bottom = 9, Left = 12 }, boxModel.Margin);
        // padding: top=8, right=4, bottom=8, left=4 (StyleRectEdges(h=4, v=8))
        Assert.Equal(new RectEdges { Top = 8, Right = 4, Bottom = 8, Left = 4 }, boxModel.Padding);
        // border: top=3, right=2, bottom=3, left=2 (Border(horizontal=2, vertical=3))
        Assert.Equal(new RectEdges { Top = 3, Right = 2, Bottom = 3, Left = 2 }, boxModel.Border);
        // content: no children
        Assert.Equal(default, boxModel.Content);
        // Boundings: size(200,100) + padding(8,16) + border(4,6) = (212, 122)
        Assert.Equal(new Size<int>(212, 122), boxModel.Boundings.Size);
    }
}
