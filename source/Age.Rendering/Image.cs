namespace Age.Rendering;

public class Image
{
    public required uint   Height { get; init; }
    public required uint[] Pixels { get; init; } = [];
    public required uint   Width  { get; init; }
}
