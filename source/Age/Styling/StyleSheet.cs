namespace Age.Styling;

public record StyleSheet
{
    public Style? Active   { get; init; }
    public Style? Base     { get; init; }
    public Style? Checked  { get; init; }
    public Style? Disabled { get; init; }
    public Style? Enabled  { get; init; }
    public Style? Focus    { get; init; }
    public Style? Hovered  { get; init; }
}
