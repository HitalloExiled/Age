using System.Runtime.CompilerServices;
using System.Text;
using Age.Numerics;
using Age.Platforms.Display;

namespace Age.Styling;

internal struct StyleData
{
    // 8-bytes
    public string? FontFamily;

    // 4-bytes
    public AlignmentKind?            Alignment;
    public Color?                    BackgroundColor;
    public Unit?                     Baseline;
    public Border?                   Border;
    public BoxSizing?                BoxSizing;
    public Color?                    Color;
    public ContentJustificationKind? ContentJustification;
    public CursorKind?               Cursor;
    public FontWeight?               FontWeight;
    public ItemsAlignmentKind?       ItemsAlignment;
    public StyleRectEdges?           Margin;
    public SizeUnit?                 MaxSize;
    public SizeUnit?                 MinSize;
    public OverflowKind?             Overflow;
    public StyleRectEdges?           Padding;
    public PositionKind?             Positioning;
    public SizeUnit?                 Size;
    public StackKind?                Stack;
    public TextAlignmentKind?        TextAlignment;
    public Transform2D?              Transform;

    // 2-bytes
    public ushort? FontSize;

    // 1-byte aligment
    public bool? Hidden;
    public bool? TextSelection;

    private static void Merge(ref StyleData target, in StyleData source, in StyleData fallback)
    {
        target.Alignment            = source.Alignment            ?? fallback.Alignment;
        target.BackgroundColor      = source.BackgroundColor      ?? fallback.BackgroundColor;
        target.Baseline             = source.Baseline             ?? fallback.Baseline;
        target.Border               = source.Border               ?? fallback.Border;
        target.BoxSizing            = source.BoxSizing            ?? fallback.BoxSizing;
        target.Color                = source.Color                ?? fallback.Color;
        target.ContentJustification = source.ContentJustification ?? fallback.ContentJustification;
        target.Cursor               = source.Cursor               ?? fallback.Cursor;
        target.FontFamily           = source.FontFamily           ?? fallback.FontFamily;
        target.FontSize             = source.FontSize             ?? fallback.FontSize;
        target.FontWeight           = source.FontWeight           ?? fallback.FontWeight;
        target.Hidden               = source.Hidden               ?? fallback.Hidden;
        target.ItemsAlignment       = source.ItemsAlignment       ?? fallback.ItemsAlignment;
        target.Margin               = source.Margin               ?? fallback.Margin;
        target.MaxSize              = source.MaxSize              ?? fallback.MaxSize;
        target.MinSize              = source.MinSize              ?? fallback.MinSize;
        target.Overflow             = source.Overflow             ?? fallback.Overflow;
        target.Padding              = source.Padding              ?? fallback.Padding;
        target.Positioning          = source.Positioning          ?? fallback.Positioning;
        target.Size                 = source.Size                 ?? fallback.Size;
        target.Stack                = source.Stack                ?? fallback.Stack;
        target.TextAlignment        = source.TextAlignment        ?? fallback.TextAlignment;
        target.TextSelection        = source.TextSelection        ?? fallback.TextSelection;
        target.Transform            = source.Transform            ?? fallback.Transform;
    }

    public static StyleProperty Diff(in StyleData left, in StyleData right)
    {
        var changes = StyleProperty.None;

        check(left.Alignment            == right.Alignment,            StyleProperty.Alignment);
        check(left.BackgroundColor      == right.BackgroundColor,      StyleProperty.BackgroundColor);
        check(left.Baseline             == right.Baseline,             StyleProperty.Baseline);
        check(left.Border               == right.Border,               StyleProperty.Border);
        check(left.BoxSizing            == right.BoxSizing,            StyleProperty.BoxSizing);
        check(left.Color                == right.Color,                StyleProperty.Color);
        check(left.ContentJustification == right.ContentJustification, StyleProperty.ContentJustification);
        check(left.Cursor               == right.Cursor,               StyleProperty.Cursor);
        check(left.FontFamily           == right.FontFamily,           StyleProperty.FontFamily);
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
        check(left.Transform            == right.Transform,            StyleProperty.Transform);

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
                if (builder.Length > 0)
                {
                    builder.Append("; ");
                }

                builder.Append($"{name}: {value}");
            }
        }

        appendProperty("Alignment",            in this.Alignment);
        appendProperty("BackgroundColor",      in this.BackgroundColor);
        appendProperty("Baseline",             in this.Baseline);
        appendProperty("Border",               in this.Border);
        appendProperty("BoxSizing",            in this.BoxSizing);
        appendProperty("Color",                in this.Color);
        appendProperty("ContentJustification", in this.ContentJustification);
        appendProperty("Cursor",               in this.Cursor);
        appendProperty("FontFamily",           in this.FontFamily);
        appendProperty("FontSize",             in this.FontSize);
        appendProperty("FontWeight",           in this.FontWeight);
        appendProperty("Hidden",               in this.Hidden);
        appendProperty("ItemsAlignment",       in this.ItemsAlignment);
        appendProperty("Margin",               in this.Margin);
        appendProperty("MaxSize",              in this.MaxSize);
        appendProperty("MinSize",              in this.MinSize);
        appendProperty("Padding",              in this.Padding);
        appendProperty("Positioning",          in this.Positioning);
        appendProperty("Size",                 in this.Size);
        appendProperty("Stack",                in this.Stack);
        appendProperty("TextAlignment",        in this.TextAlignment);
        appendProperty("TextSelection",        in this.TextSelection);
        appendProperty("Transform",            in this.Transform);

        return builder.ToString();
    }

    internal void Copy(in StyleData data, StyleProperty property)
    {
        switch (property)
        {
            case StyleProperty.Alignment:            this.Alignment            = data.Alignment;            break;
            case StyleProperty.BackgroundColor:      this.BackgroundColor      = data.BackgroundColor;      break;
            case StyleProperty.Baseline:             this.Baseline             = data.Baseline;             break;
            case StyleProperty.Border:               this.Border               = data.Border;               break;
            case StyleProperty.BoxSizing:            this.BoxSizing            = data.BoxSizing;            break;
            case StyleProperty.Color:                this.Color                = data.Color;                break;
            case StyleProperty.ContentJustification: this.ContentJustification = data.ContentJustification; break;
            case StyleProperty.Cursor:               this.Cursor               = data.Cursor;               break;
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
            case StyleProperty.Transform:            this.Transform            = data.Transform;            break;
        }
    }
}
