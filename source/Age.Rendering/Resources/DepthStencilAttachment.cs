namespace Age.Rendering.Resources;

public readonly record struct DepthStencilAttachment
{
    public readonly Texture2D Texture { get; }

    internal DepthStencilAttachment(Texture2D texture) =>
        this.Texture = texture;

    public void Dispose() =>
        this.Texture.Dispose();
}
