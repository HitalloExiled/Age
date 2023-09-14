
using System.Runtime.InteropServices;
using Age.Core.Interfaces;
using Age.Platform.Windows.Native;
using Age.Platform.Windows.Native.Types;

namespace Age.Platform.Windows;

public class WindowsLibrary(string lib) : ILibrary
{
    private readonly HMODULE handler = Kernel32.LoadLibraryW(lib);

    private bool disposed;

    public bool IsLoaded => this.handler != default;

    ~WindowsLibrary() =>
        this.Dispose(disposing: false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            Kernel32.FreeLibrary(this.handler);

            this.disposed = true;
        }
    }

    public nint GetProcAddress(string proc) =>
        Kernel32.GetProcAddress(this.handler, proc);

    public T? GetProcAddress<T>(string proc) where T : Delegate
    {
        var pointer = Kernel32.GetProcAddress(this.handler, proc);

        return pointer != default ? Marshal.GetDelegateForFunctionPointer<T>(pointer) : null;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
