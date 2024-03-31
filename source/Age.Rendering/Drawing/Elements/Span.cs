namespace Age.Rendering.Drawing.Elements;

public class Span : Element
{
    public override string NodeName { get; } = nameof(Span);

    private readonly Style defaultStyle = new()
        {
            Stack = StackMode.Horizontal,
        };
}
