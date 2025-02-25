using System.Runtime.CompilerServices;
using System.Text;
using Age.Numerics;
using Age.Platforms.Display;

namespace Age.Styling;

internal struct StyleData
{
    // 4-bytes
    public AlignmentKind?            Alignment;
    public Color?                    BackgroundColor;
    public Unit?                     Baseline;
    public Border?                   Border;
    public BoxSizing?                BoxSizing;
    public Color?                    Color;
    public ContentJustificationKind? ContentJustification;
    public CursorKind?               Cursor;
    public string?                   FontFamily;
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
}
