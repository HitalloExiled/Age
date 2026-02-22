namespace Age.Styling;

[Flags]
public enum StyleProperty
{
    None                 = 0,
    Alignment            = 1 << 0,
    BackgroundColor      = 1 << 1,
    BackgroundImage      = 1 << 2,
    Baseline             = 1 << 3,
    Border               = 1 << 4,
    BoxSizing            = 1 << 5,
    Color                = 1 << 6,
    ContentJustification = 1 << 7,
    Cursor               = 1 << 8,
    FontFamily           = 1 << 9,
    FontSize             = 1 << 10,
    FontWeight           = 1 << 11,
    ItemsAlignment       = 1 << 12,
    Margin               = 1 << 13,
    MaxSize              = 1 << 14,
    MinSize              = 1 << 15,
    Overflow             = 1 << 16,
    Padding              = 1 << 17,
    Positioning          = 1 << 18,
    Size                 = 1 << 19,
    Stack                = 1 << 20,
    TextAlignment        = 1 << 21,
    TextSelection        = 1 << 22,
    Transforms           = 1 << 23,
    TransformOrigin      = 1 << 24,
    ZIndex               = 1 << 25,

    All = Alignment
        | BackgroundColor
        | BackgroundImage
        | Baseline
        | Border
        | BoxSizing
        | Color
        | ContentJustification
        | Cursor
        | FontFamily
        | FontSize
        | FontWeight
        | ItemsAlignment
        | Margin
        | MaxSize
        | MinSize
        | Overflow
        | Padding
        | Positioning
        | Size
        | Stack
        | TextAlignment
        | TextSelection
        | Transforms
        | TransformOrigin
        | ZIndex
}
