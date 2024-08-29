#if !Windows
    #define Windows
#endif

#if Windows
using System.Runtime.InteropServices;
using Age.Platforms.Windows.Native;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms;

public partial class Library(string lib)
{
    private readonly HMODULE handler = Kernel32.LoadLibraryW(lib);

    private nint PlatformGetProcAddress(string proc) =>
        Kernel32.GetProcAddress(this.handler, proc);

    private T? PlatformGetProcAddress<T>(string proc) where T : Delegate
    {
        var pointer = Kernel32.GetProcAddress(this.handler, proc);

        return pointer != default ? Marshal.GetDelegateForFunctionPointer<T>(pointer) : null;
    }

    private void PlatformDispose(bool disposing)
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

    private bool PlatformIsLoaded() =>
        this.handler != default;
}
#endif
