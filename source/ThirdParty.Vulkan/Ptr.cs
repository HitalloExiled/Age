using System.Diagnostics;

namespace ThirdParty.Vulkan;

[DebuggerDisplay("{Value}")]
public unsafe struct Ptr<T>(T* value) where T : unmanaged
{
    public T* Value = value;

    public static implicit operator T*(Ptr<T> prt) => prt.Value;
    public static implicit operator Ptr<T>(T* pointer) => new(pointer);
}
