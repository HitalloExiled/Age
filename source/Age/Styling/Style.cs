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

    public void Append(in StyleData source) =>
        Merge(ref this, source, this);
}

public record Style
{
    internal event Action<StyleProperty>? Changed;

    internal StyleData Data;

    public AlignmentKind? Alignment
    {
        get => this.Data.Alignment;
        set => this.Set(ref this.Data.Alignment, value, StyleProperty.Alignment);
    }

    public Color? BackgroundColor
    {
        get => this.Data.BackgroundColor;
        set => this.Set(ref this.Data.BackgroundColor, value, StyleProperty.BackgroundColor);
    }

    public Unit? Baseline
    {
        get => this.Data.Baseline;
        set => this.Set(ref this.Data.Baseline, value, StyleProperty.Baseline);
    }

    public Border? Border
    {
        get => this.Data.Border;
        set => this.Set(ref this.Data.Border, value, StyleProperty.Border);
    }

    public BoxSizing? BoxSizing
    {
        get => this.Data.BoxSizing;
        set => this.Set(ref this.Data.BoxSizing, value, StyleProperty.BoxSizing);
    }

    public Color? Color
    {
        get => this.Data.Color;
        set => this.Set(ref this.Data.Color, value, StyleProperty.Color);
    }

    public ContentJustificationKind? ContentJustification
    {
        get => this.Data.ContentJustification;
        set => this.Set(ref this.Data.ContentJustification, value, StyleProperty.ContentJustification);
    }

    public string? FontFamily
    {
        get => this.Data.FontFamily;
        set => this.Set(ref this.Data.FontFamily, value, StyleProperty.FontFamily);
    }

    public ushort? FontSize
    {
        get => this.Data.FontSize;
        set => this.Set(ref this.Data.FontSize, value, StyleProperty.FontSize);
    }

    public bool? Hidden
    {
        get => this.Data.Hidden;
        set => this.Set(ref this.Data.Hidden, value, StyleProperty.Hidden);
    }

    public ItemsAlignmentKind? ItemsAlignment
    {
        get => this.Data.ItemsAlignment;
        set => this.Set(ref this.Data.ItemsAlignment, value, StyleProperty.ItemsAlignment);
    }

    public RectEdges? Margin
    {
        get => this.Data.Margin;
        set => this.Set(ref this.Data.Margin, value, StyleProperty.Margin);
    }

    public SizeUnit? MaxSize
    {
        get => this.Data.MaxSize;
        set => this.Set(ref this.Data.MaxSize, value, StyleProperty.MaxSize);
    }

    public SizeUnit? MinSize
    {
        get => this.Data.MinSize;
        set => this.Set(ref this.Data.MinSize, value, StyleProperty.MinSize);
    }

    public RectEdges? Padding
    {
        get => this.Data.Padding;
        set => this.Set(ref this.Data.Padding, value, StyleProperty.Padding);
    }

    public Point<float>? Pivot
    {
        get => this.Data.Pivot;
        set => this.Set(ref this.Data.Pivot, value, StyleProperty.Pivot);
    }

    public Point<int>? Position
    {
        get => this.Data.Position;
        set => this.Set(ref this.Data.Position, value, StyleProperty.Position);
    }

    public PositionKind? Positioning
    {
        get => this.Data.Positioning;
        set => this.Set(ref this.Data.Positioning, value, StyleProperty.Positioning);
    }

    public float? Rotation
    {
        get => this.Data.Rotation;
        set => this.Set(ref this.Data.Rotation, value, StyleProperty.Rotation);
    }

    public SizeUnit? Size
    {
        get => this.Data.Size;
        set => this.Set(ref this.Data.Size, value, StyleProperty.Size);
    }

    public StackKind? Stack
    {
        get => this.Data.Stack;
        set => this.Set(ref this.Data.Stack, value, StyleProperty.Stack);
    }

    public TextAlignmentKind? TextAlignment
    {
        get => this.Data.TextAlignment;
        set => this.Set(ref this.Data.TextAlignment, value, StyleProperty.TextAlignment);
    }

    private Style(StyleData data) =>
        this.Data = data;

    public Style() { }

    public static Style Merge(Style left, Style right) =>
        new(StyleData.Merge(left.Data, right.Data));

    private void Set<T>(ref T? field, T? value, StyleProperty property)
    {
        if (!Equals(field, value))
        {
            field = value;

            Changed?.Invoke(property);
        }
    }

    public void Append(Style source) =>
        this.Data.Append(source.Data);
}
