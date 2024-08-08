using Age.Numerics;

namespace Age.Styling;

public partial record Style
{
    public event Action? Changed;

    private Point<int>?    align;
    private AlignmentType? alignment;
    private Color?         backgroundColor;
    private float?         baseline;
    private Border?        border;
    private BoxSizing?     boxSizing;
    private Color?         color;
    private string?        fontFamily;
    private ushort?        fontSize;
    private Margin?        margin;
    private Size<uint>?    maxSize;
    private Size<uint>?    minSize;
    private Point<float>?  pivot;
    private Point<int>?    position;
    private PositionType?  positionType;
    private float?         rotation;
    private Size<uint>?    size;
    private StackType?     stack;

    public Point<int>? Align
    {
        get => this.align;
        set => this.Set(ref this.align, value);
    }

    public AlignmentType? Alignment
    {
        get => this.alignment;
        set => this.Set(ref this.alignment, value);
    }

    public Color? BackgroundColor
    {
        get => this.backgroundColor;
        set => this.Set(ref this.backgroundColor, value);
    }

    public float? Baseline
    {
        get => this.baseline;
        set => this.Set(ref this.baseline, value);
    }

    public Border? Border
    {
        get => this.border;
        set => this.Set(ref this.border, value);
    }

    public BoxSizing? BoxSizing
    {
        get => this.boxSizing;
        set => this.Set(ref this.boxSizing, value);
    }

    public Color? Color
    {
        get => this.color;
        set => this.Set(ref this.color, value);
    }

    public string? FontFamily
    {
        get => this.fontFamily;
        set => this.Set(ref this.fontFamily, value);
    }

    public ushort? FontSize
    {
        get => this.fontSize;
        set => this.Set(ref this.fontSize, value);
    }

    public Margin? Margin
    {
        get => this.margin;
        set => this.Set(ref this.margin, value);
    }

    public Size<uint>? MaxSize
    {
        get => this.maxSize;
        set => this.Set(ref this.maxSize, value);
    }

    public Size<uint>? MinSize
    {
        get => this.minSize;
        set => this.Set(ref this.minSize, value);
    }

    public Point<float>? Pivot
    {
        get => this.pivot;
        set => this.Set(ref this.pivot, value);
    }

    public Point<int>? Position
    {
        get => this.position;
        set => this.Set(ref this.position, value);
    }

    public PositionType? PositionType
    {
        get => this.positionType;
        set => this.Set(ref this.positionType, value);
    }

    public float? Rotation
    {
        get => this.rotation;
        set => this.Set(ref this.rotation, value);
    }

    public Size<uint>? Size
    {
        get => this.size;
        set => this.Set(ref this.size, value);
    }

    public StackType? Stack
    {
        get => this.stack;
        set => this.Set(ref this.stack, value);
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
