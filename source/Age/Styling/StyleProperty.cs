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
    Hidden               = 1 << 12,
    ItemsAlignment       = 1 << 13,
    Margin               = 1 << 14,
    MaxSize              = 1 << 15,
    MinSize              = 1 << 16,
    Overflow             = 1 << 17,
    Padding              = 1 << 18,
    Positioning          = 1 << 19,
    Size                 = 1 << 20,
    Stack                = 1 << 21,
    TextAlignment        = 1 << 22,
    TextSelection        = 1 << 23,
    Transforms           = 1 << 24,
    TransformOrigin      = 1 << 25,
    ZIndex               = 1 << 26,

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
        | Hidden
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
