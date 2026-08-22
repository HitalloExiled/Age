using Age.Styling;

namespace Age.Tests.Age.Elements;

public class ElementStylingTest
{
    [Fact]
    public void Focus_SetsIsFocused()
    {
        var element = new TestElement();

        element.Focus();

        Assert.True(element.IsFocused);
    }

    [Fact]
    public void Blur_ClearsIsFocused()
    {
        var element = new TestElement();

        element.Focus();
        element.Blur();

        Assert.False(element.IsFocused);
    }

    [Fact]
    public void Blur_WithoutFocus_DoesNotThrow()
    {
        var element = new TestElement();

        var exception = Record.Exception(element.Blur);

        Assert.Null(exception);
    }

    [Fact]
    public void Click_SetsIsActive()
    {
        var element = new TestElement();

        element.Click();

        Assert.True(element.IsActive);
    }

    [Fact]
    public void InvokeDeactivate_ClearsIsActive()
    {
        var element = new TestElement();

        element.Click();
        element.InvokeDeactivate();

        Assert.False(element.IsActive);
    }

    [Fact]
    public void IsFocusableSetter_DoesNotThrow()
    {
        var element = new TestElement();

        var exception = Record.Exception(() => element.SetIsFocusable(true));

        Assert.Null(exception);
    }

    [Fact]
    public void IsScrollableSetter_DoesNotThrow()
    {
        var element = new TestElement();

        var exception = Record.Exception(() => element.SetIsScrollable(true));

        Assert.Null(exception);
    }

    [Fact]
    public void ComputedStyle_AfterConnect_IsNotNull()
    {
        var element = new TestElement();

        element.Connect();

        Assert.NotNull(element.ComputedStyle);
    }

    [Fact]
    public void Style_Property_GetterSetter_Works()
    {
        var element = new TestElement();
        var style  = new Style { Alignment = Alignment.Center };

        element.Style = style;

        Assert.Same(style, element.Style);
    }

    [Fact]
    public void CanScrollX_WithOverflowScrollX_ReturnsTrue()
    {
        var element = new TestElement();

        element.ComputedStyle.Overflow = Overflow.ScrollX;

        Assert.True(element.CanScrollX);
    }

    [Fact]
    public void CanScrollY_WithOverflowScrollY_ReturnsTrue()
    {
        var element = new TestElement();

        element.ComputedStyle.Overflow = Overflow.ScrollY;

        Assert.True(element.CanScrollY);
    }

    [Fact]
    public void CanScroll_WithOverflowScroll_ReturnsTrue()
    {
        var element = new TestElement();

        element.ComputedStyle.Overflow = Overflow.Scroll;

        Assert.True(element.CanScroll);
    }

    [Fact]
    public void CanScroll_WithoutOverflow_ReturnsFalse()
    {
        var element = new TestElement();

        Assert.False(element.CanScroll);
    }

}
