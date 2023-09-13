namespace Age.Platform.Windows.Display;

public class WindowManager : IDisposable
{
    private readonly Dictionary<HWND, Window> windows = new();

    private bool disposed;

    ~WindowManager() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                foreach (var window in this.windows.Values)
                {
                    window.Close();
                }

                this.windows.Clear();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Window CreateWindow(string title, int width, int height, int x, int y)
    {
        var window = new Window(title, width, height, x, y);

        window.Destroyed += this.DestroyWindow;

        this.windows.Add(window.Handle, window);

        return window;
    }

    public void DestroyWindow(Window window)
    {
        window.Destroyed -= this.DestroyWindow;

        window.Close();

        this.windows.Remove(window.Handle);
    }

    public void ProcessEvents()
    {
        foreach (var window in this.windows.Values)
        {
            window.DoEvents();
        }
    }
}
