namespace Age.Core.Interfaces;

public interface ILibrary : IDisposable
{
    bool IsLoaded { get; }
    nint GetProcAddress(string proc);
    T? GetProcAddress<T>(string proc) where T : Delegate;
}
