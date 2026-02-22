namespace Age.Rendering.Resources;

public readonly record struct DepthStencilAttachment
{
    internal ImageLayout FinalLayout { get; }

    public Texture2D Texture { get; }

    internal DepthStencilAttachment(Texture2D texture, ImageLayout finalLayout)
    {
        this.Texture     = texture;
        this.FinalLayout = finalLayout;
    }

    public void Dispose() =>
        this.Texture.Dispose();
}
