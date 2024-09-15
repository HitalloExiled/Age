using Age.Elements.Layouts;

namespace Age.Elements;

public class Span : Element
{
    internal override BoxLayout Layout { get; }

    public override string NodeName { get; } = nameof(Span);

    public Span() =>
        this.Layout = new(this) { IsInline = true };
}
