namespace Age.Rendering.Resources;

public class RenderPipeline : Resource
{
    public required RenderPass  RenderPass  { get; init; }
    public required Framebuffer Framebuffer { get; init; }

    protected override void Disposed()
    {
        this.RenderPass.Dispose();
        this.Framebuffer.Dispose();
    }
}
