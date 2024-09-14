using Age.Numerics;

namespace Age.Styling;

public partial record Style
{
    public event Action? Changed;

    private AlignmentKind?     alignment;
    private Color?             backgroundColor;
    private float?             baseline;
    private Border?            border;
    private BoxSizing?         boxSizing;
    private Color?             color;
    private string?            fontFamily;
    private ushort?            fontSize;
    private Margin?            margin;
    private SizeUnit?          maxSize;
    private SizeUnit?          minSize;
    private Margin?            padding;
    private Point<float>?      pivot;
    private Point<int>?        position;
    private PositionType?      positionType;
    private float?             rotation;
    private SizeUnit?          size;
    private StackKind?         stack;
    private TextAlignmentKind? textAlignment;

    public AlignmentKind? Alignment
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

    public SizeUnit? MaxSize
    {
        get => this.maxSize;
        set => this.Set(ref this.maxSize, value);
    }

    public SizeUnit? MinSize
    {
        get => this.minSize;
        set => this.Set(ref this.minSize, value);
    }

    public Margin? Padding
    {
        get => this.padding;
        set => this.Set(ref this.padding, value);
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

    public SizeUnit? Size
    {
        get => this.size;
        set => this.Set(ref this.size, value);
    }

    public StackKind? Stack
    {
        get => this.stack;
        set => this.Set(ref this.stack, value);
    }

    public TextAlignmentKind? TextAlignment
    {
        get => this.textAlignment;
        set => this.Set(ref this.textAlignment, value);
    }

    public static Style Merge(Style left, Style right) =>
        new()
        {
            Alignment       = left.Alignment       ?? right.Alignment,
            BackgroundColor = left.BackgroundColor ?? right.BackgroundColor,
            Baseline        = left.Baseline        ?? right.Baseline,
            Border          = left.Border          ?? right.Border,
            BoxSizing       = left.BoxSizing       ?? right.BoxSizing,
            Color           = left.Color           ?? right.Color,
            FontFamily      = left.FontFamily      ?? right.FontFamily,
            FontSize        = left.FontSize        ?? right.FontSize,
            Margin          = left.Margin          ?? right.Margin,
            MaxSize         = left.MaxSize         ?? right.MaxSize,
            MinSize         = left.MinSize         ?? right.MinSize,
            Padding         = left.Padding         ?? right.Padding,
            Pivot           = left.Pivot           ?? right.Pivot,
            Position        = left.Position        ?? right.Position,
            PositionType    = left.PositionType    ?? right.PositionType,
            Rotation        = left.Rotation        ?? right.Rotation,
            Size            = left.Size            ?? right.Size,
            Stack           = left.Stack           ?? right.Stack,
            TextAlignment   = left.TextAlignment   ?? right.TextAlignment,
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
