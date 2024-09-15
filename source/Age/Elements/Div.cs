using Age.Styling;

namespace Age.Elements;

public class Div : Element
{
    public override string NodeName { get; } = nameof(Div);

    public Div() =>
        this.Style.Stack = StackKind.Vertical;
}
