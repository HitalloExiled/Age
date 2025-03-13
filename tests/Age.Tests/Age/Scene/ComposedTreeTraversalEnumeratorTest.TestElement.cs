using Age.Elements;

namespace Age.Tests.Age.Scene;

#pragma warning disable CA1001

public partial class ComposedTreeTraversalEnumeratorTest
{
    public class TestElement : Element
{
    public override string NodeName { get; } = nameof(TestElement);
}
}
