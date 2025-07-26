using Age.Core.Interfaces;
using Age.Numerics;
using Age.Platforms.Display;

namespace Age.Styling;

public record Style : IPoolable
{
    internal event Action<StyleProperty>? PropertyChanged;

    private StyleData data;

    internal StyleData Data => this.data;

    public Alignment? Alignment
    {
        get => this.data.Alignment;
        set => this.Set(ref this.data.Alignment, value, StyleProperty.Alignment);
    }

    public Color? BackgroundColor
    {
        get => this.data.BackgroundColor;
        set => this.Set(ref this.data.BackgroundColor, value, StyleProperty.BackgroundColor);
    }

    public Image? BackgroundImage
    {
        get => this.data.BackgroundImage;
        set => this.Set(ref this.data.BackgroundImage, value, StyleProperty.BackgroundImage);
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

    public ContentJustification? ContentJustification
    {
        get => this.data.ContentJustification;
        set => this.Set(ref this.data.ContentJustification, value, StyleProperty.ContentJustification);
    }

    public Cursor? Cursor
    {
        get => this.data.Cursor;
        set => this.Set(ref this.data.Cursor, value, StyleProperty.Cursor);
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

    public FontWeight? FontWeight
    {
        get => this.data.FontWeight;
        set => this.Set(ref this.data.FontWeight, value, StyleProperty.FontWeight);
    }

    public bool? Hidden
    {
        get => this.data.Hidden;
        set => this.Set(ref this.data.Hidden, value, StyleProperty.Hidden);
    }

    public ItemsAlignment? ItemsAlignment
    {
        get => this.data.ItemsAlignment;
        set => this.Set(ref this.data.ItemsAlignment, value, StyleProperty.ItemsAlignment);
    }

    public StyleRectEdges? Margin
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

    public Overflow? Overflow
    {
        get => this.data.Overflow;
        set => this.Set(ref this.data.Overflow, value, StyleProperty.Overflow);
    }

    public StyleRectEdges? Padding
    {
        get => this.data.Padding;
        set => this.Set(ref this.data.Padding, value, StyleProperty.Padding);
    }

    public Positioning? Positioning
    {
        get => this.data.Positioning;
        set => this.Set(ref this.data.Positioning, value, StyleProperty.Positioning);
    }

    public SizeUnit? Size
    {
        get => this.data.Size;
        set => this.Set(ref this.data.Size, value, StyleProperty.Size);
    }

    public StackDirection? StackDirection
    {
        get => this.data.Stack;
        set => this.Set(ref this.data.Stack, value, StyleProperty.Stack);
    }

    public TextAlignment? TextAlignment
    {
        get => this.data.TextAlignment;
        set => this.Set(ref this.data.TextAlignment, value, StyleProperty.TextAlignment);
    }

    public bool? TextSelection
    {
        get => this.data.TextSelection;
        set => this.Set(ref this.data.TextSelection, value, StyleProperty.TextSelection);
    }

    public TransformOp[]? Transforms
    {
        get => this.data.Transforms;
        set => this.Set(ref this.data.Transforms, value, StyleProperty.Transforms);
    }

    public PointUnit? TransformOrigin
    {
        get => this.data.TransformOrigin;
        set => this.Set(ref this.data.TransformOrigin, value, StyleProperty.TransformOrigin);
    }
    public int? ZIndex
    {
        get => this.data.ZIndex;
        set => this.Set(ref this.data.ZIndex, value, StyleProperty.ZIndex);
    }

    private Style(StyleData data) =>
        this.data = data;

    public Style() { }

    public static Style Merge(Style left, Style right) =>
        new(StyleData.Merge(left.data, right.data));

    void IPoolable.Reset() =>
        this.Clear();

    private void Set<T>(ref T? field, T? value, StyleProperty property)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;

            PropertyChanged?.Invoke(property);
        }
    }

    internal void Clear() =>
        this.data = default;

    internal void Copy(Style source) =>
        this.data = source.data;

    internal void Copy(in StyleData source) =>
        this.data = source;

    internal void Copy(Style source, StyleProperty property) =>
        this.data.Copy(source.data, property);

    internal void Merge(Style source) =>
        this.data.Merge(source.data);

    internal void Merge(in StyleData source) =>
        this.data.Merge(source);

    public override string ToString() =>
        this.data.ToString();
}
