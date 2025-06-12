using Age.Numerics;
using Age.Platforms.Display;
using System.Runtime.CompilerServices;
using System.Text;

namespace Age.Styling;

internal struct StyleData
{
    #region 8-bytes
    /// <see cref="StyleProperty.BackgroundImage">
    public Image? BackgroundImage;

    /// <see cref="StyleProperty.Border">
    public Border? Border;

    /// <see cref="StyleProperty.FontFamily">
    public string? FontFamily;

    /// <see cref="StyleProperty.Margin">
    public StyleRectEdges? Margin;

    /// <see cref="StyleProperty.Padding">
    public StyleRectEdges? Padding;

    /// <see cref="StyleProperty.Transforms">
    public TransformOp[]? Transforms;
    #endregion

    #region 4-bytes
    /// <see cref="StyleProperty.BackgroundColor">
    public Color? BackgroundColor;

    /// <see cref="StyleProperty.Baseline">
    public Unit? Baseline;


    /// <see cref="StyleProperty.Color">
    public Color? Color;

    /// <see cref="StyleProperty.MaxSize">
    public SizeUnit? MaxSize;

    /// <see cref="StyleProperty.MinSize">
    public SizeUnit? MinSize;

    /// <see cref="StyleProperty.Size">
    public SizeUnit? Size;

    /// <see cref="StyleProperty.TransformOrigin">
    public PointUnit? TransformOrigin;
    #endregion

    #region 2-bytes
    /// <see cref="StyleProperty.FontSize">
    public ushort? FontSize;

    /// <see cref="StyleProperty.FontWeight">
    public FontWeight? FontWeight;
    #endregion

    #region 1-byte aligment
    /// <see cref="StyleProperty.Alignment">
    public Alignment? Alignment;

    /// <see cref="StyleProperty.BoxSizing">
    public BoxSizing? BoxSizing;

    /// <see cref="StyleProperty.ContentJustification">
    public ContentJustification? ContentJustification;

    /// <see cref="StyleProperty.Cursor">
    public Cursor? Cursor;

    /// <see cref="StyleProperty.FontFeature">
    public FontFeature? FontFeature;

    /// <see cref="StyleProperty.Hidden">
    public bool? Hidden;

    /// <see cref="StyleProperty.ItemsAlignment">
    public ItemsAlignment? ItemsAlignment;

    /// <see cref="StyleProperty.Overflow">
    public Overflow? Overflow;

    /// <see cref="StyleProperty.Positioning">
    public Positioning? Positioning;

    /// <see cref="StyleProperty.Stack">
    public StackDirection? Stack;

    /// <see cref="StyleProperty.TextAlignment">
    public TextAlignment? TextAlignment;

    /// <see cref="StyleProperty.TextSelection">
    public bool? TextSelection;

    #endregion

    private static void Merge(ref StyleData target, in StyleData left, in StyleData right)
    {
        target.Alignment            = left.Alignment            ?? right.Alignment;
        target.BackgroundColor      = left.BackgroundColor      ?? right.BackgroundColor;
        target.BackgroundImage      = left.BackgroundImage      ?? right.BackgroundImage;
        target.Baseline             = left.Baseline             ?? right.Baseline;
        target.Border               = left.Border               ?? right.Border;
        target.BoxSizing            = left.BoxSizing            ?? right.BoxSizing;
        target.Color                = left.Color                ?? right.Color;
        target.ContentJustification = left.ContentJustification ?? right.ContentJustification;
        target.Cursor               = left.Cursor               ?? right.Cursor;
        target.FontFamily           = left.FontFamily           ?? right.FontFamily;
        target.FontFeature          = left.FontFeature          ?? right.FontFeature;
        target.FontSize             = left.FontSize             ?? right.FontSize;
        target.FontWeight           = left.FontWeight           ?? right.FontWeight;
        target.Hidden               = left.Hidden               ?? right.Hidden;
        target.ItemsAlignment       = left.ItemsAlignment       ?? right.ItemsAlignment;
        target.Margin               = left.Margin               ?? right.Margin;
        target.MaxSize              = left.MaxSize              ?? right.MaxSize;
        target.MinSize              = left.MinSize              ?? right.MinSize;
        target.Overflow             = left.Overflow             ?? right.Overflow;
        target.Padding              = left.Padding              ?? right.Padding;
        target.Positioning          = left.Positioning          ?? right.Positioning;
        target.Size                 = left.Size                 ?? right.Size;
        target.Stack                = left.Stack                ?? right.Stack;
        target.TextAlignment        = left.TextAlignment        ?? right.TextAlignment;
        target.TextSelection        = left.TextSelection        ?? right.TextSelection;
        target.Transforms           = left.Transforms           ?? right.Transforms;
        target.TransformOrigin      = left.TransformOrigin      ?? right.TransformOrigin;
    }

    public static StyleProperty Diff(in StyleData left, in StyleData right)
    {
        var changes = StyleProperty.None;

        check(left.Alignment            == right.Alignment,            StyleProperty.Alignment);
        check(left.BackgroundColor      == right.BackgroundColor,      StyleProperty.BackgroundColor);
        check(left.BackgroundImage      == right.BackgroundImage,      StyleProperty.BackgroundImage);
        check(left.Baseline             == right.Baseline,             StyleProperty.Baseline);
        check(left.Border               == right.Border,               StyleProperty.Border);
        check(left.BoxSizing            == right.BoxSizing,            StyleProperty.BoxSizing);
        check(left.Color                == right.Color,                StyleProperty.Color);
        check(left.ContentJustification == right.ContentJustification, StyleProperty.ContentJustification);
        check(left.Cursor               == right.Cursor,               StyleProperty.Cursor);
        check(left.FontFamily           == right.FontFamily,           StyleProperty.FontFamily);
        check(left.FontFeature          == right.FontFeature,          StyleProperty.FontFeature);
        check(left.FontSize             == right.FontSize,             StyleProperty.FontSize);
        check(left.FontWeight           == right.FontWeight,           StyleProperty.FontWeight);
        check(left.Hidden               == right.Hidden,               StyleProperty.Hidden);
        check(left.ItemsAlignment       == right.ItemsAlignment,       StyleProperty.ItemsAlignment);
        check(left.Margin               == right.Margin,               StyleProperty.Margin);
        check(left.MaxSize              == right.MaxSize,              StyleProperty.MaxSize);
        check(left.MinSize              == right.MinSize,              StyleProperty.MinSize);
        check(left.Overflow             == right.Overflow,             StyleProperty.Overflow);
        check(left.Padding              == right.Padding,              StyleProperty.Padding);
        check(left.Positioning          == right.Positioning,          StyleProperty.Positioning);
        check(left.Size                 == right.Size,                 StyleProperty.Size);
        check(left.Stack                == right.Stack,                StyleProperty.Stack);
        check(left.TextAlignment        == right.TextAlignment,        StyleProperty.TextAlignment);
        check(left.TextSelection        == right.TextSelection,        StyleProperty.TextSelection);
        check(left.Transforms            == right.Transforms,            StyleProperty.Transforms);
        check(left.TransformOrigin      == right.TransformOrigin,      StyleProperty.TransformOrigin);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void check(bool isEqual, StyleProperty property)
        {
            if (!isEqual)
            {
                changes |= property;
            }
        }

        return changes;
    }

    public static StyleData Merge(in StyleData left, in StyleData right)
    {
        Unsafe.SkipInit(out StyleData target);

        Merge(ref target, left, right);

        return target;
    }

    public void Merge(in StyleData source) =>
        Merge(ref this, source, this);

    public override readonly string ToString()
    {
        var builder = new StringBuilder();

        void appendProperty<T>(string name, in T? value)
        {
            if (value != null)
            {
                builder.Append($"{name}: {value}");
                builder.Append("; ");
            }
        }

        appendProperty(nameof(StyleProperty.Alignment),            in this.Alignment);
        appendProperty(nameof(StyleProperty.BackgroundColor),      in this.BackgroundColor);
        appendProperty(nameof(StyleProperty.BackgroundImage),      in this.BackgroundImage);
        appendProperty(nameof(StyleProperty.Baseline),             in this.Baseline);
        appendProperty(nameof(StyleProperty.Border),               in this.Border);
        appendProperty(nameof(StyleProperty.BoxSizing),            in this.BoxSizing);
        appendProperty(nameof(StyleProperty.Color),                in this.Color);
        appendProperty(nameof(StyleProperty.ContentJustification), in this.ContentJustification);
        appendProperty(nameof(StyleProperty.Cursor),               in this.Cursor);
        appendProperty(nameof(StyleProperty.FontFamily),           in this.FontFamily);
        appendProperty(nameof(StyleProperty.FontFeature),          in this.FontFeature);
        appendProperty(nameof(StyleProperty.FontSize),             in this.FontSize);
        appendProperty(nameof(StyleProperty.FontWeight),           in this.FontWeight);
        appendProperty(nameof(StyleProperty.Hidden),               in this.Hidden);
        appendProperty(nameof(StyleProperty.ItemsAlignment),       in this.ItemsAlignment);
        appendProperty(nameof(StyleProperty.Margin),               in this.Margin);
        appendProperty(nameof(StyleProperty.MaxSize),              in this.MaxSize);
        appendProperty(nameof(StyleProperty.MinSize),              in this.MinSize);
        appendProperty(nameof(StyleProperty.Overflow),             in this.Overflow);
        appendProperty(nameof(StyleProperty.Padding),              in this.Padding);
        appendProperty(nameof(StyleProperty.Positioning),          in this.Positioning);
        appendProperty(nameof(StyleProperty.Size),                 in this.Size);
        appendProperty(nameof(StyleProperty.Stack),                in this.Stack);
        appendProperty(nameof(StyleProperty.TextAlignment),        in this.TextAlignment);
        appendProperty(nameof(StyleProperty.TextSelection),        in this.TextSelection);
        appendProperty(nameof(StyleProperty.Transforms),            in this.Transforms);
        appendProperty(nameof(StyleProperty.TransformOrigin),      in this.TransformOrigin);

        if (builder.Length > 0)
        {
            builder.Remove(builder.Length - 2, 2);
        }

        return builder.ToString();
    }

    internal void Copy(in StyleData data, StyleProperty property)
    {
        switch (property)
        {
            case StyleProperty.Alignment:            this.Alignment            = data.Alignment;            break;
            case StyleProperty.BackgroundColor:      this.BackgroundColor      = data.BackgroundColor;      break;
            case StyleProperty.BackgroundImage:      this.BackgroundImage      = data.BackgroundImage;      break;
            case StyleProperty.Baseline:             this.Baseline             = data.Baseline;             break;
            case StyleProperty.Border:               this.Border               = data.Border;               break;
            case StyleProperty.BoxSizing:            this.BoxSizing            = data.BoxSizing;            break;
            case StyleProperty.Color:                this.Color                = data.Color;                break;
            case StyleProperty.ContentJustification: this.ContentJustification = data.ContentJustification; break;
            case StyleProperty.Cursor:               this.Cursor               = data.Cursor;               break;
            case StyleProperty.FontFeature:          this.FontFeature          = data.FontFeature;           break;
            case StyleProperty.FontFamily:           this.FontFamily           = data.FontFamily;           break;
            case StyleProperty.FontSize:             this.FontSize             = data.FontSize;             break;
            case StyleProperty.FontWeight:           this.FontWeight           = data.FontWeight;           break;
            case StyleProperty.Hidden:               this.Hidden               = data.Hidden;               break;
            case StyleProperty.ItemsAlignment:       this.ItemsAlignment       = data.ItemsAlignment;       break;
            case StyleProperty.Margin:               this.Margin               = data.Margin;               break;
            case StyleProperty.MaxSize:              this.MaxSize              = data.MaxSize;              break;
            case StyleProperty.MinSize:              this.MinSize              = data.MinSize;              break;
            case StyleProperty.Overflow:             this.Overflow             = data.Overflow;             break;
            case StyleProperty.Padding:              this.Padding              = data.Padding;              break;
            case StyleProperty.Positioning:          this.Positioning          = data.Positioning;          break;
            case StyleProperty.Size:                 this.Size                 = data.Size;                 break;
            case StyleProperty.Stack:                this.Stack                = data.Stack;                break;
            case StyleProperty.TextAlignment:        this.TextAlignment        = data.TextAlignment;        break;
            case StyleProperty.TextSelection:        this.TextSelection        = data.TextSelection;        break;
            case StyleProperty.Transforms:            this.Transforms            = data.Transforms;            break;
            case StyleProperty.TransformOrigin:      this.TransformOrigin      = data.TransformOrigin;      break;
        }
    }
}
