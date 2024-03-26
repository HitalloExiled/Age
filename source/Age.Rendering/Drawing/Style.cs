using Age.Numerics;

namespace Age.Rendering.Drawing;

public record Style
{
    public Color        Color      { get; set; }
    public string       FontFamily { get; set; } = "Segoe UI";
    public ushort       FontSize   { get; set; } = 12;
    public Point<int>   Position   { get; set; }
    public Size<uint>   Size       { get; set; }
    public BorderStyle? Border     { get; set; }
    public StackMode    Stack      { get; set; }
}
