namespace Age.Elements;

internal class LayerTree : IDisposable
{
    public object Lockpad { get; } = new();

    private Layer? root;
    private bool disposed;

    public Layer? Root
    {
        get => this.root;
        set
        {
            if (this.root != value)
            {
                if (this.root != null)
                {
                    this.root.Tree = null;
                }

                if (value != null)
                {
                    value.Tree = this;
                }

                this.root = value;
            }
        }
    }

    public bool IsDirty { get; set; }

    ~LayerTree() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                if (this.Root != null)
                {
                    this.Root.Dispose();

                    foreach (var layer in this.Root.Traverse())
                    {
                        layer.Dispose();
                    }
                }
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Update()
    {
        if (this.IsDirty)
        {
            if (this.Root != null)
            {
                this.Root.Update();

                foreach (var layer in this.Root.Traverse())
                {
                    layer.Update();
                }
            }

            this.IsDirty = false;
        }
    }
}
