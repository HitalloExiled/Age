using Age.Elements;

namespace Age.Tests.Age.Elements;

#pragma warning disable CA1001

public partial class ComposedTreeTraversalEnumeratorTest
{
    public class NestedHostElement : Element
{
    public override string NodeName { get; } = nameof(NestedHostElement);

    public Element[] ShadowNodes { get; }

    public NestedHostElement(string name)
    {
        this.AttachShadowTree();

        TestElement nestedChild0;
        TestElement nestedChild1;
        TestElement nestedChild11;
        TestElement nestedChild12;
        TestElement nestedChild13;

        this.Name = name;

        this.ShadowTree.Children =
        [
            nestedChild0 = new TestElement($"{name}.#.1")
            {
                Children =
                [],
            },
            nestedChild1 = new TestElement($"{name}.#.2")
            {
                Children =
                [
                    nestedChild11 = new TestElement($"{name}.#.2.1"),
                    nestedChild12 = new TestElement($"{name}.#.2.2"),
                    nestedChild13 = new TestElement($"{name}.#.2.3"),
                ],
            },
        ];

        this.ShadowNodes =
        [
            nestedChild0,
            nestedChild1,
            nestedChild11,
            nestedChild12,
            nestedChild13,
        ];
    }
}
}
