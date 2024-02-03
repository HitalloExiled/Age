using System.Diagnostics;
using Age.Core.Interop;

namespace ThirdParty.Vulkan;

public abstract unsafe partial record NativeReference
{
    [DebuggerDisplay("{Handle}")]
    protected unsafe struct Ref<T> where T : unmanaged
    {
        public T* Handle;

        public Ref() =>
            this.Handle = PointerHelper.Alloc(new T());

        public Ref(in T value) =>
            this.Handle = PointerHelper.Alloc(value);

        public readonly void Free() =>
            PointerHelper.Free(this.Handle);

        public static implicit operator T*(Ref<T> @ref) => @ref.Handle;
        public static implicit operator Ref<T>(in T value) => new(value);
    }
}
