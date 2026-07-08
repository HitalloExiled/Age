using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeArray
{
    public struct Enumerator<T>(UnsafeArray* array) where T : unmanaged
    {
        private T* current;
        private int index;
        private readonly UnsafeArray* array = array;

        public bool MoveNext()
        {
            if ((uint)this.index < (uint)this.array->length)
            {
                this.current = (T*)this.array->buffer + this.index;
                this.index++;
                return true;
            }

            this.current = default;
            return false;
        }

        public void Reset()
        {
            this.index = 0;
            this.current = default;
        }

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(this.current != null);
                return *this.current;
            }
        }

        public readonly void Dispose()
        { }
    }
}
