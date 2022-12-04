using RSG = Age.Servers.RenderingServerGlobals;

namespace Age.Servers.Rendering;

#pragma warning disable IDE0060,IDE0059,IDE0052,CS0414 // TODO - Remove

internal class RenderingServerDefault : RenderingServer
{
    public static new RenderingServer Singleton { get; private set; } = null!;

    private readonly Queue<Action> commandQueue = new();
    private readonly bool createThread = false;

    private int changes = 0;

    public override bool HasChanged { get; }

    public override bool IsRenderLoopEnabled { get; }

    public RenderingServerDefault(bool createThread)
    {
        Singleton = this;
        base.Init();
    }

    private void InternalDraw(bool swapBuffers, double frameStep)
    {
        // TODO - servers\rendering\rendering_server_default.cpp[73]

        this.changes = 0;

        RSG.Rasterizer.BeginFrame(frameStep);

        // TODO - servers\rendering\rendering_server_default.cpp[79]

        var timeUsec = DateTime.Now.Ticks;

        RSG.Scene.Update();

        RSG.Viewport.DrawViewports();
        RSG.Rasterizer.EndFrame(frameStep);
    }

    public override void Draw(bool swapBuffers, double frameStep)
    {
        if (this.createThread)
        {
            this.commandQueue.Enqueue(() => this.InternalDraw(swapBuffers, frameStep));
        }
        else
        {
            this.InternalDraw(swapBuffers, frameStep);
        }
    }

    public override void Init() => throw new NotImplementedException();
    public override void Sync() => throw new NotImplementedException();
}
