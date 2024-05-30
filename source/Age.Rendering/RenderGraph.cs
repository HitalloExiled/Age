namespace Age.Rendering;

public class RenderGraph : IDisposable
{
    private bool disposed;

    public required string Name { get; init; }

    public required RenderGraphPass[] Passes { get; init; }

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
}
