using System.Runtime.CompilerServices;
using Age.Numerics;

namespace Age.Styling;

internal struct StyleData
{
    // 4-bytes aligment
    public AlignmentKind?            Alignment;
    public Color?                    BackgroundColor;
    public Unit?                     Baseline;
    public Border?                   Border;
    public BoxSizing?                BoxSizing;
    public Color?                    Color;
    public ContentJustificationKind? ContentJustification;
    public string?                   FontFamily;
    public ItemsAlignmentKind?       ItemsAlignment;
    public RectEdges?                Margin;
    public SizeUnit?                 MaxSize;
    public SizeUnit?                 MinSize;
    public OverflowKind?             Overflow;
    public RectEdges?                Padding;
    public PositionKind?             Positioning;
    public SizeUnit?                 Size;
    public StackKind?                Stack;
    public TextAlignmentKind?        TextAlignment;
    public Transform2D?              Transform;

    // 2-bytes aligment
    public ushort? FontSize;

    // 1-byte aligment
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
        target.Transform            = source.Transform            ?? fallback.Transform;
        target.Positioning          = source.Positioning          ?? fallback.Positioning;
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
