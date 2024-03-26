namespace Age.Rendering.Drawing;

public class Content : Element
{
    public Content() =>
        this.Style = new()
        {
            Stack = StackMode.Vertical,
        };
}
