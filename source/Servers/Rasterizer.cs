namespace Age.Servers;

#pragma warning disable IDE0052 // TODO - Remove

internal class Rasterizer
{
    private int frame;
    private double delta;
    private double time;

    public static Rasterizer Singleton { get; } = new();

    public int FrameNumber => this.frame;

    public void BeginFrame(double frameStep)
    {
        this.frame++;
		this.delta = frameStep;
		this.time += frameStep;
    }

    public void EndFrame(double scale) => Console.WriteLine((this, scale));
}
