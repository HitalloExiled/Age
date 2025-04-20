using Age.Core;

namespace Age.Platforms;

public sealed partial class Library : Disposable
{
    public bool IsLoaded => this.PlatformIsLoaded();

    protected override void OnDisposed(bool disposing) =>
        this.PlatformDispose(disposing);

    public nint GetProcAddress(string proc) =>
        this.PlatformGetProcAddress(proc);

    public T? GetProcAddress<T>(string proc) where T : Delegate =>
        this.PlatformGetProcAddress<T>(proc);
}
