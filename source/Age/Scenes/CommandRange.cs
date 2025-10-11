namespace Age.Scenes;

public readonly partial record struct CommandRange(CommandRange.Variant Color, CommandRange.Variant Encode)
{
    public CommandRange WithClamp(ushort color, ushort encode) =>
        new(this.Color.WithClamp(color), this.Encode.WithClamp(encode));

    public CommandRange WithOffset(short color, short encode) =>
        new(this.Color.WithOffset(color), this.Encode.WithOffset(encode));

    public CommandRange WithPreEnd(ushort color, ushort encode) =>
        new(this.Color.WithPreEnd(color), this.Encode.WithPreEnd(encode));

    public CommandRange WithPost(ushort color, ushort encode) =>
        new(this.Color.WithPost(color), this.Encode.WithPost(encode));

    public CommandRange WithPostEnd(ushort color, ushort encode) =>
        new(this.Color.WithPostEnd(color), this.Encode.WithPostEnd(encode));

    public CommandRange WithPostOffset(short color, short encode) =>
        new(this.Color.WithPostOffset(color), this.Encode.WithPostOffset(encode));

    public CommandRange WithPostResize(short color, short encode) =>
        new(this.Color.WithPostResize(color), this.Encode.WithPostResize(encode));

    public CommandRange WithPostStart(ushort color, ushort encode) =>
        new(this.Color.WithPostStart(color), this.Encode.WithPostStart(encode));

    public override string ToString() =>
        $"Color: {this.Color}, Encode: {this.Encode}";
}
