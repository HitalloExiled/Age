using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Core.Collections;

public unsafe partial struct UnsafeList
{
    public struct Enumerator<T> where T : unmanaged
    {
        private T* current;

        private readonly int count;
        private readonly int offset;

        private int index = -1;
        private UnsafeBuffer buffer;

        internal Enumerator(UnsafeBuffer buffer, int offset, int count)
        {
            this.count  = count;
            this.offset = offset;
            this.buffer = buffer;
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

        public void Dispose()
        {
            this.index = -1;
            this.current = default;
        }

        public bool MoveNext()
        {
            this.index++;

            if (this.index == this.count)
            {
                this.index = 0;
                this.current = default;
                return false;
            }

            var capacity = this.buffer.Length;
            var arrayIndex = this.offset + this.index;

            if (arrayIndex >= capacity)
            {
                arrayIndex -= capacity;
            }

            this.current = this.buffer.Element<T>(arrayIndex);
            return true;
        }

        public void Reset()
        {
            this.index = -1;
            this.current = default;
        }
    }
}
