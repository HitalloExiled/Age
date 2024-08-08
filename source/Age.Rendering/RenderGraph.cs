using System.Diagnostics.CodeAnalysis;

namespace Age.Rendering;

public class RenderGraph : IDisposable
{
    private static RenderGraph? active;

    public static RenderGraph Active
    {
        get => active ?? throw new InvalidOperationException("There no active RenderGraph");
        set => active = value;
    }

    private bool disposed;

    public required string Name { get; init; }

    public required RenderGraphPass[] Passes { get; init; }

    public bool HasStarted { get; private set; }

    public bool Disabled { get; set; }

    public T GetRenderGraphPass<T>() where T : RenderGraphPass =>
        this.TryGetRenderGraphPass<T>(out var renderPass)
            ? renderPass
            : throw new InvalidOperationException($"Can't find any {nameof(T)} on {this.Name} RenderGraph");

    public bool TryGetRenderGraphPass<T>([NotNullWhen(true)] out T? renderPass) where T : RenderGraphPass
    {
        foreach (var pass in this.Passes)
        {
            if (pass is T requestedPass)
            {
                renderPass = requestedPass;

                return true;
            }
        }

        renderPass = null;

        return false;
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
        this.HasStarted = true;

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
