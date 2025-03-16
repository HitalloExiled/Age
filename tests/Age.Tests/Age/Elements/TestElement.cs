using Age.Elements;

namespace Age.Tests.Age.Elements;

public class TestElement : Element
{
    public override string NodeName { get; } = nameof(TestElement);

    public TestElement(string name) => this.Name = name;
}
