namespace Age.Resources;

public readonly record struct ColorAttachment
{
    private readonly Texture2D  color;
    private readonly Texture2D? resolve;

    public readonly bool HasResolve => this.resolve != null;

    public readonly Texture2D Texture => this.resolve ?? this.color;

    internal ColorAttachment(Texture2D target, Texture2D? resolve)
    {
        this.color   = target;
        this.resolve = resolve;
    }

    public void Dispose()
    {
        this.color.Dispose();
        this.resolve?.Dispose();
    }
}
