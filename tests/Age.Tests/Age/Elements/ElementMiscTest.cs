namespace Age.Tests.Age.Elements;

public class ElementMiscTest
{
    [Fact]
    public void ActivatedEvent_InvokeActivate_Fires()
    {
        var element = new TestElement();
        var fired   = false;

        element.Activated += () => fired = true;
        element.InvokeActivate();

        Assert.True(fired);
    }

    [Fact]
    public void DeactivatedEvent_InvokeDeactivate_Fires()
    {
        var element = new TestElement();
        var fired   = false;

        element.Deactivated += () => fired = true;
        element.InvokeDeactivate();

        Assert.True(fired);
    }

    [Fact]
    public void FocusedEvent_Focus_Fires()
    {
        var element = new TestElement();
        var fired   = false;

        element.Focused += (in _) => fired = true;
        element.Focus();

        Assert.True(fired);
    }

    [Fact]
    public void BluredEvent_Blur_Fires()
    {
        var element = new TestElement();
        var fired   = false;

        element.Focus();

        element.Blured += (in _) => fired = true;
        element.Blur();

        Assert.True(fired);
    }

    [Fact]
    public void ClickedEvent_Click_Fires()
    {
        var element = new TestElement();
        var fired   = false;

        element.Clicked += (in _) => fired = true;
        element.Click();

        Assert.True(fired);
    }

    [Fact]
    public void Event_AddAndRemove_Works()
    {
        var element     = new TestElement();
        var fired       = false;
        void handler() => fired = true;

        element.Activated += handler;
        element.Activated -= handler;
        element.InvokeActivate();

        Assert.False(fired);
    }

    [Fact]
    public void Event_MultipleHandlers_Fire()
    {
        var element      = new TestElement();
        var firedCount   = 0;
        void handler1() => firedCount++;
        void handler2() => firedCount++;

        element.Activated += handler1;
        element.Activated += handler2;
        element.InvokeActivate();

        Assert.Equal(2, firedCount);
    }
}
