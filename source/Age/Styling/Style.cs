using System.Runtime.CompilerServices;
using Age.Numerics;

namespace Age.Styling;

internal struct StyleData
{
    // 96-bytes
    public Border? Border;

    // 32-bytes
    public RectEdges? Margin;
    public RectEdges? Padding;

    // 16-bytes
    public Color?    BackgroundColor;
    public Color?    Color;
    public SizeUnit? MaxSize;
    public SizeUnit? MinSize;
    public SizeUnit? Size;

    // 8-bytes
    public Unit?         Baseline;
    public Point<float>? Pivot;
    public Point<int>?   Position;

    // 4-bytes
    public AlignmentKind?            Alignment;
    public BoxSizing?                BoxSizing;
    public ContentJustificationKind? ContentJustification;
    public string?                   FontFamily;
    public ItemsAlignmentKind?       ItemsAlignment;
    public PositionKind?             Positioning;
    public float?                    Rotation;
    public StackKind?                Stack;
    public TextAlignmentKind?        TextAlignment;

    // 2-bytes
    public ushort? FontSize;

    // 1-byte
    public bool? Hidden;

    private static void Merge(ref StyleData target, in StyleData source, in StyleData fallback)
    {
        target.Alignment            = source.Alignment            ?? fallback.Alignment;
        target.BackgroundColor      = source.BackgroundColor      ?? fallback.BackgroundColor;
        target.Baseline             = source.Baseline             ?? fallback.Baseline;
        target.Border               = source.Border               ?? fallback.Border;
        target.BoxSizing            = source.BoxSizing            ?? fallback.BoxSizing;
        target.Color                = source.Color                ?? fallback.Color;
        target.ContentJustification = source.ContentJustification ?? fallback.ContentJustification;
        target.FontFamily           = source.FontFamily           ?? fallback.FontFamily;
        target.FontSize             = source.FontSize             ?? fallback.FontSize;
        target.Hidden               = source.Hidden               ?? fallback.Hidden;
        target.ItemsAlignment       = source.ItemsAlignment       ?? fallback.ItemsAlignment;
        target.Margin               = source.Margin               ?? fallback.Margin;
        target.MaxSize              = source.MaxSize              ?? fallback.MaxSize;
        target.MinSize              = source.MinSize              ?? fallback.MinSize;
        target.Padding              = source.Padding              ?? fallback.Padding;
        target.Pivot                = source.Pivot                ?? fallback.Pivot;
        target.Position             = source.Position             ?? fallback.Position;
        target.Positioning          = source.Positioning          ?? fallback.Positioning;
        target.Rotation             = source.Rotation             ?? fallback.Rotation;
        target.Size                 = source.Size                 ?? fallback.Size;
        target.Stack                = source.Stack                ?? fallback.Stack;
        target.TextAlignment        = source.TextAlignment        ?? fallback.TextAlignment;
    }

    public static StyleData Merge(in StyleData left, in StyleData right)
    {
        Unsafe.SkipInit(out StyleData target);

        Merge(ref target, left, right);

        return target;
    }

    public void Merge(in StyleData source) =>
        Merge(ref this, source, this);
}

public record Style
{
    internal event Action<StyleProperty>? Changed;

    private StyleData data;

    public AlignmentKind? Alignment
    {
        get => this.data.Alignment;
        set => this.Set(ref this.data.Alignment, value, StyleProperty.Alignment);
    }

    public Color? BackgroundColor
    {
        get => this.data.BackgroundColor;
        set => this.Set(ref this.data.BackgroundColor, value, StyleProperty.BackgroundColor);
    }

    public Unit? Baseline
    {
        get => this.data.Baseline;
        set => this.Set(ref this.data.Baseline, value, StyleProperty.Baseline);
    }

    public Border? Border
    {
        get => this.data.Border;
        set => this.Set(ref this.data.Border, value, StyleProperty.Border);
    }

    public BoxSizing? BoxSizing
    {
        get => this.data.BoxSizing;
        set => this.Set(ref this.data.BoxSizing, value, StyleProperty.BoxSizing);
    }

    public Color? Color
    {
        get => this.data.Color;
        set => this.Set(ref this.data.Color, value, StyleProperty.Color);
    }

    public ContentJustificationKind? ContentJustification
    {
        get => this.data.ContentJustification;
        set => this.Set(ref this.data.ContentJustification, value, StyleProperty.ContentJustification);
    }

    public string? FontFamily
    {
        get => this.data.FontFamily;
        set => this.Set(ref this.data.FontFamily, value, StyleProperty.FontFamily);
    }

    public ushort? FontSize
    {
        get => this.data.FontSize;
        set => this.Set(ref this.data.FontSize, value, StyleProperty.FontSize);
    }

    public bool? Hidden
    {
        get => this.data.Hidden;
        set => this.Set(ref this.data.Hidden, value, StyleProperty.Hidden);
    }

    public ItemsAlignmentKind? ItemsAlignment
    {
        get => this.data.ItemsAlignment;
        set => this.Set(ref this.data.ItemsAlignment, value, StyleProperty.ItemsAlignment);
    }

    public RectEdges? Margin
    {
        get => this.data.Margin;
        set => this.Set(ref this.data.Margin, value, StyleProperty.Margin);
    }

    public SizeUnit? MaxSize
    {
        get => this.data.MaxSize;
        set => this.Set(ref this.data.MaxSize, value, StyleProperty.MaxSize);
    }

    public SizeUnit? MinSize
    {
        get => this.data.MinSize;
        set => this.Set(ref this.data.MinSize, value, StyleProperty.MinSize);
    }

    public RectEdges? Padding
    {
        get => this.data.Padding;
        set => this.Set(ref this.data.Padding, value, StyleProperty.Padding);
    }

    public Point<float>? Pivot
    {
        get => this.data.Pivot;
        set => this.Set(ref this.data.Pivot, value, StyleProperty.Pivot);
    }

    public Point<int>? Position
    {
        get => this.data.Position;
        set => this.Set(ref this.data.Position, value, StyleProperty.Position);
    }

    public PositionKind? Positioning
    {
        get => this.data.Positioning;
        set => this.Set(ref this.data.Positioning, value, StyleProperty.Positioning);
    }

    public float? Rotation
    {
        get => this.data.Rotation;
        set => this.Set(ref this.data.Rotation, value, StyleProperty.Rotation);
    }

    public SizeUnit? Size
    {
        get => this.data.Size;
        set => this.Set(ref this.data.Size, value, StyleProperty.Size);
    }

    public StackKind? Stack
    {
        get => this.data.Stack;
        set => this.Set(ref this.data.Stack, value, StyleProperty.Stack);
    }

    public TextAlignmentKind? TextAlignment
    {
        get => this.data.TextAlignment;
        set => this.Set(ref this.data.TextAlignment, value, StyleProperty.TextAlignment);
    }

    private Style(StyleData data) =>
        this.data = data;

    public Style() { }

    public static Style Merge(Style left, Style right) =>
        new(StyleData.Merge(left.data, right.data));

    private void Set<T>(ref T? field, T? value, StyleProperty property)
    {
        if (!Equals(field, value))
        {
            field = value;

            Changed?.Invoke(property);
        }
    }

    public void Clear() =>
        this.data = default;

    public void Copy(Style source) =>
        this.data = source.data;

    public void Merge(Style source) =>
        this.data.Merge(source.data);
}
