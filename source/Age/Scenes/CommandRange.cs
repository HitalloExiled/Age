namespace Age.Scenes;

public readonly partial record struct CommandRange(CommandRange.Variant Color, CommandRange.Variant Encode)
{
    public CommandRange WithClamp(int color, int encode) =>
        new(this.Color.WithClamp(color), this.Encode.WithClamp(encode));

    public CommandRange WithOffset(int color, int encode) =>
        new(this.Color.WithOffset(color), this.Encode.WithOffset(encode));

    public CommandRange WithPreEnd(int color, int encode) =>
        new(this.Color.WithPreEnd(color), this.Encode.WithPreEnd(encode));

    public CommandRange WithPost(int color, int encode) =>
        new(this.Color.WithPost(color), this.Encode.WithPost(encode));

    public CommandRange WithPostEnd(int color, int encode) =>
        new(this.Color.WithPostEnd(color), this.Encode.WithPostEnd(encode));

    public CommandRange WithPostOffset(int color, int encode) =>
        new(this.Color.WithPostOffset(color), this.Encode.WithPostOffset(encode));

    public CommandRange WithPostResize(int color, int encode) =>
        new(this.Color.WithPostResize(color), this.Encode.WithPostResize(encode));

    public CommandRange WithPostStart(int color, int encode) =>
        new(this.Color.WithPostStart(color), this.Encode.WithPostStart(encode));

    public override string ToString() =>
        $"Color: {this.Color}, Encode: {this.Encode}";
}
