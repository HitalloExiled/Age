using System.Runtime.InteropServices;

namespace Age.Core;

[StructLayout(LayoutKind.Auto)]
public readonly struct CacheValue<T>(T value)
{
    private readonly uint version = CacheTracker.Version;

    public readonly T Value = value;

    public readonly bool IsInvalid => this.version != CacheTracker.Version;
}
