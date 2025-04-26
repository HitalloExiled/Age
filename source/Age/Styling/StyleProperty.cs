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
    FontFeature          = 1 << 10,
    FontSize             = 1 << 11,
    FontWeight           = 1 << 12,
    Hidden               = 1 << 13,
    ItemsAlignment       = 1 << 14,
    Margin               = 1 << 15,
    MaxSize              = 1 << 16,
    MinSize              = 1 << 17,
    Overflow             = 1 << 18,
    Padding              = 1 << 19,
    Positioning          = 1 << 20,
    Size                 = 1 << 21,
    Stack                = 1 << 22,
    TextAlignment        = 1 << 23,
    TextSelection        = 1 << 24,
    Transform            = 1 << 25,

    All = Alignment | BackgroundColor | BackgroundImage | Baseline | Border | BoxSizing | Color | ContentJustification | Cursor | FontFamily | FontFeature | FontSize | FontWeight | Hidden | ItemsAlignment | Margin | MaxSize | MinSize | Overflow | Padding | Positioning | Size | Stack | TextAlignment | TextSelection | Transform
}


