namespace Age.Scenes;

public readonly partial record struct CommandRange(CommandRange.Segment Color, CommandRange.Segment Index)
{
    public CommandRange WithExtend(ushort color, ushort index) =>
        new(this.Color.WithExtend(color), this.Index.WithExtend(index));

    public CommandRange WithEnd(ushort color, ushort index) =>
        new(this.Color.WithEnd(color), this.Index.WithEnd(index));

    public CommandRange WithExtendOffset(short color, short index) =>
        new(this.Color.WithExtendOffset(color), this.Index.WithExtendOffset(index));

    public CommandRange WithOffset(short colorOffset, short indexOffset) =>
        new(this.Color.WithOffset(colorOffset), this.Index.WithOffset(indexOffset));

    public override string ToString() =>
        $"Color: {this.Color}, Index: {this.Index}";
}
