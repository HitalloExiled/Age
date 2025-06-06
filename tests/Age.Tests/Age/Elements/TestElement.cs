using Age.Elements;

namespace Age.Tests.Age.Elements;

public class TestElement : Element
{
    public override string NodeName => nameof(TestElement);

    public TestElement(string name) => this.Name = name;
}
