using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeStack
{
    public struct Enumerator<T> where T : unmanaged
    {
        private T* current;
        private int index;
        private readonly int count;
        private readonly UnsafeBuffer buffer;

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(this.current != null);

                return *this.current;
            }
        }

        internal Enumerator(UnsafeBuffer buffer, int count)
        {
            this.index   = count;
            this.count   = count;
            this.buffer  = buffer;
            this.current = default;
        }

        public void Dispose()
        {
            this.index   = this.count;
            this.current = default;
        }

        public bool MoveNext()
        {
            this.index--;

            if (this.index < 0)
            {
                this.current = default;
                return false;
            }

            this.current = this.buffer.Element<T>(this.index);
            return true;
        }

        public void Reset()
        {
            this.index   = this.count;
            this.current = default;
        }
    }
}
