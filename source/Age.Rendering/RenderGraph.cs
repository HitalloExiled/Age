using Age.Rendering.Resources;

namespace Age.Rendering;

public class RenderGraph : IDisposable
{
    private bool disposed;

    public static RenderGraph? Active { get; set; }

    public required string Name { get; init; }

    public required RenderGraphPass[] Passes { get; init; }

    public bool Disabled { get; set; }

    public RenderPass? GetRenderPass<T>() where T : RenderGraphPass
    {
        foreach (var pass in this.Passes)
        {
            if (pass is T)
            {
                return pass.RenderPass;
            }
        }

        return null;
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;

            foreach (var pass in this.Passes)
            {
                pass.Dispose();
            }
        }

        GC.SuppressFinalize(this);
    }

    public void Execute()
    {
        if (!this.Disabled)
        {
            foreach (var pass in this.Passes)
            {
                if (!pass.Disabled)
                {
                    pass.Execute();
                }
            }
        }
    }

    public void Recreate()
    {
        foreach (var pass in this.Passes)
        {
            pass.Recreate();
            pass.NotifyRecreated();
        }
    }
}
