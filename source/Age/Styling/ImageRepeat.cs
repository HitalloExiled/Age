namespace Age.Styling;

public enum ImageRepeat
{
    NoRepeat = 0,
    RepeatX  = 1 << 0,
    RepeatY  = 1 << 1,
    Repeat   = RepeatX | RepeatY,
}
