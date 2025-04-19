namespace Age.Styling;

[Flags]
public enum StyleProperty
{
    None                 = 0,
    Alignment            = 1 << 0,
    BackgroundColor      = 1 << 1,
    Baseline             = 1 << 2,
    Border               = 1 << 3,
    BoxSizing            = 1 << 4,
    Color                = 1 << 5,
    ContentJustification = 1 << 6,
    Cursor               = 1 << 7,
    FontFamily           = 1 << 8,
    FontSize             = 1 << 9,
    FontWeight           = 1 << 10,
    Hidden               = 1 << 11,
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
    Transform            = 1 << 23,

    All = Alignment | BackgroundColor | Baseline | Border | BoxSizing | Color | ContentJustification | Cursor | FontFamily | FontSize | FontWeight | Hidden | ItemsAlignment | Margin | MaxSize | MinSize | Overflow | Padding | Positioning | Size | Stack | TextAlignment | TextSelection | Transform
}


