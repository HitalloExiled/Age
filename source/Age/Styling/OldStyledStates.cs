namespace Age.Styling;

public record OldStyledStates
{
    public OldStyle? Active   { get; init; }
    public OldStyle? Base     { get; init; }
    public OldStyle? Checked  { get; init; }
    public OldStyle? Disabled { get; init; }
    public OldStyle? Enabled  { get; init; }
    public OldStyle? Focus    { get; init; }
    public OldStyle? Hovered  { get; init; }
}
