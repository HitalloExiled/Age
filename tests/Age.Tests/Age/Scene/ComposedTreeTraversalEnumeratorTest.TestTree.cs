using Age.Scene;

namespace Age.Tests.Age.Scene;

#pragma warning disable CA1001

public partial class ComposedTreeTraversalEnumeratorTest
{
    public class TestTree : NodeTree
{
    protected override void Disposed(bool disposing) => throw new NotImplementedException();
}
}
