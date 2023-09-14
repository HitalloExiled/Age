
using System.Runtime.InteropServices;
using Age.Core.Interfaces;
using Age.Platform.Windows.Native;

namespace Godot.Net.Platforms.Windows;

public class WindowsLibrary : ILibrary
{
    private bool disposed;

    private readonly HMODULE handler;

    public bool IsLoaded => this.handler != default;

    public WindowsLibrary(string lib) =>
        this.handler = Kernel32.LoadLibraryW(lib);

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
