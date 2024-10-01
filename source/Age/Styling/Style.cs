using Age.Numerics;

namespace Age.Styling;

public partial record Style
{
    public event Action<StyleProperty>? Changed;

    // 96-bytes
    private Border? border;

    // 32-bytes
    private RectEdges? margin;
    private RectEdges? padding;

    // 16-bytes
    private Color?    backgroundColor;
    private Color?    color;
    private SizeUnit? maxSize;
    private SizeUnit? minSize;
    private SizeUnit? size;

    // 8-bytes
    private Unit?         baseline;
    private Point<float>? pivot;
    private Point<int>?   position;

    // 4-bytes
    private AlignmentKind?            alignment;
    private BoxSizing?                boxSizing;
    private ContentJustificationKind? contentJustification;
    private string?                   fontFamily;
    private ItemsAlignmentKind?       itemsAlignment;
    private PositionKind?             positioning;
    private float?                    rotation;
    private StackKind?                stack;
    private TextAlignmentKind?        textAlignment;

    private bool? hidden;

    // 2-bytes
    private ushort? fontSize;

    public AlignmentKind? Alignment
    {
        get => this.alignment;
        set => this.Set(ref this.alignment, value, StyleProperty.Alignment);
    }

    public Color? BackgroundColor
    {
        get => this.backgroundColor;
        set => this.Set(ref this.backgroundColor, value, StyleProperty.BackgroundColor);
    }

    public Unit? Baseline
    {
        get => this.baseline;
        set => this.Set(ref this.baseline, value, StyleProperty.Baseline);
    }

    public Border? Border
    {
        get => this.border;
        set => this.Set(ref this.border, value, StyleProperty.Border);
    }

    public BoxSizing? BoxSizing
    {
        get => this.boxSizing;
        set => this.Set(ref this.boxSizing, value, StyleProperty.BoxSizing);
    }

    public Color? Color
    {
        get => this.color;
        set => this.Set(ref this.color, value, StyleProperty.Color);
    }

    public ContentJustificationKind? ContentJustification
    {
        get => this.contentJustification;
        set => this.Set(ref this.contentJustification, value, StyleProperty.ContentJustification);
    }

    public string? FontFamily
    {
        get => this.fontFamily;
        set => this.Set(ref this.fontFamily, value, StyleProperty.FontFamily);
    }

    public ushort? FontSize
    {
        get => this.fontSize;
        set => this.Set(ref this.fontSize, value, StyleProperty.FontSize);
    }

    public bool? Hidden
    {
        get => this.hidden;
        set => this.Set(ref this.hidden, value, StyleProperty.Hidden);
    }

    public ItemsAlignmentKind? ItemsAlignment
    {
        get => this.itemsAlignment;
        set => this.Set(ref this.itemsAlignment, value, StyleProperty.ItemsAlignment);
    }

    public RectEdges? Margin
    {
        get => this.margin;
        set => this.Set(ref this.margin, value, StyleProperty.Margin);
    }

    public SizeUnit? MaxSize
    {
        get => this.maxSize;
        set => this.Set(ref this.maxSize, value, StyleProperty.MaxSize);
    }

    public SizeUnit? MinSize
    {
        get => this.minSize;
        set => this.Set(ref this.minSize, value, StyleProperty.MinSize);
    }

    public RectEdges? Padding
    {
        get => this.padding;
        set => this.Set(ref this.padding, value, StyleProperty.Padding);
    }

    public Point<float>? Pivot
    {
        get => this.pivot;
        set => this.Set(ref this.pivot, value, StyleProperty.Pivot);
    }

    public Point<int>? Position
    {
        get => this.position;
        set => this.Set(ref this.position, value, StyleProperty.Position);
    }

    public PositionKind? Positioning
    {
        get => this.positioning;
        set => this.Set(ref this.positioning, value, StyleProperty.Positioning);
    }

    public float? Rotation
    {
        get => this.rotation;
        set => this.Set(ref this.rotation, value, StyleProperty.Rotation);
    }

    public SizeUnit? Size
    {
        get => this.size;
        set => this.Set(ref this.size, value, StyleProperty.Size);
    }

    public StackKind? Stack
    {
        get => this.stack;
        set => this.Set(ref this.stack, value, StyleProperty.Stack);
    }

    public TextAlignmentKind? TextAlignment
    {
        get => this.textAlignment;
        set => this.Set(ref this.textAlignment, value, StyleProperty.TextAlignment);
    }

    public static Style Merge(Style left, Style right) =>
        new()
        {
            alignment            = left.alignment            ?? right.alignment,
            backgroundColor      = left.backgroundColor      ?? right.backgroundColor,
            baseline             = left.baseline             ?? right.baseline,
            border               = left.border               ?? right.border,
            boxSizing            = left.boxSizing            ?? right.boxSizing,
            color                = left.color                ?? right.color,
            contentJustification = left.contentJustification ?? right.contentJustification,
            fontFamily           = left.fontFamily           ?? right.fontFamily,
            fontSize             = left.fontSize             ?? right.fontSize,
            itemsAlignment       = left.itemsAlignment       ?? right.itemsAlignment,
            margin               = left.margin               ?? right.margin,
            maxSize              = left.maxSize              ?? right.maxSize,
            minSize              = left.minSize              ?? right.minSize,
            padding              = left.padding              ?? right.padding,
            pivot                = left.pivot                ?? right.pivot,
            position             = left.position             ?? right.position,
            positioning          = left.positioning          ?? right.positioning,
            rotation             = left.rotation             ?? right.rotation,
            size                 = left.size                 ?? right.size,
            stack                = left.stack                ?? right.stack,
            textAlignment        = left.textAlignment        ?? right.textAlignment,
        };

    private void Set<T>(ref T? field, T? value, StyleProperty property)
    {
        if (!Equals(field, value))
        {
            field = value;

            Changed?.Invoke(property);
        }
    }
}
