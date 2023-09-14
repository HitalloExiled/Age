using System.Diagnostics.CodeAnalysis;
using Age.Core;
using Age.Vulkan.Interfaces;
using Godot.Net.Platforms.Windows;

namespace Age.Platform.Windows.Vulkan;

public class WindowsVulkanLoader : IVulkanLoader
{
    private bool disposed;
    private readonly WindowsLibrary library = new("vulkan-1.dll");

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            this.library.Dispose();

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public T Load<T>(string name) where T : Delegate =>
        this.TryLoad<T>(name, out var proc) ? proc : throw new LoadSymbolException(name);

    public bool TryLoad<T>(string name, [NotNullWhen(true)]out T? result) where T : Delegate
    {
        result = this.library.GetProcAddress<T>(name);

        return result != null;
    }
}
