using Age.Numerics;

namespace Age.Rendering.Drawing.Styling;

public record Style
{
    private AlignmentType? alignment;
    private Color?         backgroundColor;
    private float?         baseline;
    private Color?         borderColor;
    private uint?          borderRadius;
    private uint?          borderSize;
    private Color?         color;
    private string?        fontFamily;
    private ushort?        fontSize;
    private Size<int>?     maxSize;
    private Size<int>?     minSize;
    private Point<int>?    position;
    private PositionType?  positionType;
    private Size<int>?     size;
    private StackType?     stack;

    public event Action? Changed;

    public AlignmentType? Alignment
    {
        get => alignment;
        set => Set(ref alignment, value);
    }

    public Color? BackgroundColor
    {
        get => backgroundColor;
        set => Set(ref backgroundColor, value);
    }

    public float? Baseline
    {
        get => baseline;
        set => Set(ref baseline, value);
    }

    public Color? BorderColor
    {
        get => this.borderColor;
        set => Set(ref this.borderColor, value);
    }

    public uint? BorderSize
    {
        get => this.borderSize;
        set => Set(ref this.borderSize, value);
    }

    public uint? BorderRadius
    {
        get => this.borderRadius;
        set => Set(ref this.borderRadius, value);
    }

    public Color? BackgroundColor1
    {
        get => this.backgroundColor;
        set => Set(ref this.backgroundColor, value);
    }

    public Color? Color
    {
        get => color;
        set => Set(ref color, value);
    }

    public string? FontFamily
    {
        get => fontFamily;
        set => Set(ref fontFamily, value);
    }

    public ushort? FontSize
    {
        get => fontSize;
        set => Set(ref fontSize, value);
    }

    public Size<int>? MaxSize
    {
        get => maxSize;
        set => Set(ref maxSize, value);
    }

    public Size<int>? MinSize
    {
        get => minSize;
        set => Set(ref minSize, value);
    }

    public Point<int>? Position
    {
        get => position;
        set => Set(ref position, value);
    }

    public PositionType? PositionType
    {
        get => positionType;
        set => Set(ref positionType, value);
    }

    public Size<int>? Size
    {
        get => size;
        set => Set(ref size, value);
    }

    public StackType? Stack
    {
        get => stack;
        set => Set(ref stack, value);
    }

    public static Style Merge(Style left, Style right) =>
        new()
        {
            Alignment    = left.Alignment    ?? right.Alignment,
            FontFamily   = left.FontFamily   ?? right.FontFamily,
            FontSize     = left.FontSize     ?? right.FontSize,
            MaxSize      = left.MaxSize      ?? right.MaxSize,
            MinSize      = left.MinSize      ?? right.MinSize,
            Position     = left.Position     ?? right.Position,
            PositionType = left.PositionType ?? right.PositionType,
            Size         = left.Size         ?? right.Size,
        };

    private void Set<T>(ref T? field, T? value)
    {
        if (!Equals(field, value))
        {
            field = value;

            Changed?.Invoke();
        }
    }
}
