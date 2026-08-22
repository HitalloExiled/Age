using Age.Elements;

namespace Age.Tests.Age.Elements;

public class TestElement : Element
{
    public override string NodeName => nameof(TestElement);

    public TestElement()
    {
    }

    public TestElement(string name) => this.Name = name;

    public void SetIsFocusable(bool value)  => this.IsFocusable  = value;
    public void SetIsScrollable(bool value) => this.IsScrollable = value;
}
