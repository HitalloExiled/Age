namespace Age.Platforms.Display;

public enum Cursor : byte
{
    Arrow,
    Busy,
    Cross,
    DiagonalResizeNESW,
    DiagonalResizeNWSE,
    Drag,
    Drop,
    Forbidden,
    Hand,
    Help,
    HorizontalResize,
    HorizontalSplit,
    Move,
    Progress,
    Text,
    Unavailable,
    VerticalResize,
    VerticalSplit,
    Wait
}

public static class CursorExtensions
{
    extension(Cursor)
    {
        public static int Length => 17;
    }
}
