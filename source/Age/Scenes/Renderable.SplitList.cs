using System.Diagnostics;
using Age.Core.Extensions;

namespace Age.Scenes;

public abstract partial class Renderable<T>
{
    [DebuggerTypeProxy(typeof(Renderable<>.SplitList.DebugView))]
    internal partial struct SplitList
    {
        private T[] items = [];

        public int Count { get; private set; }

        public readonly T this[uint index]
        {
            get => this.AsSpan()[(int)index];
            set => this.AsSpan()[(int)index] = value;
        }

        public readonly T this[int index]
        {
            get => this.AsSpan()[index];
            set => this.AsSpan()[index] = value;
        }

        public int Separator
        {
            get;
            set
            {
                if (value > this.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                field = value;
            }
        } = -1;

        public readonly Span<T> Pre  => this.Separator > -1 ? this.items.AsSpan(0, this.Separator) : this.AsSpan();
        public readonly Span<T> Post => this.Separator > -1 ? this.items.AsSpan(this.Separator, this.Count - this.Separator) : [];

        public readonly int Capacity => this.items.Length;

        public SplitList() { }

        public void EnsureCapacity(int capacity)
        {
            if (capacity > this.items.Length)
            {
                Array.Resize(ref this.items, capacity);
            }
        }

        private void EnsureCapacity()
        {
            if (this.Count + 1 > this.items.Length)
            {
                var capacity = int.Min(this.items.Length == 0 ? 4 : this.Count * 2, int.MaxValue);

                Array.Resize(ref this.items, capacity);
            }
        }

        public void Add(T item)
        {
            this.EnsureCapacity();

            this.items[this.Count] = item;

            this.Count++;
        }

        public void AddPre(T item)
        {
            if (this.Separator == this.Count || this.Separator < 0)
            {
                this.Add(item);

                this.Separator = this.Count;
            }
            else
            {
                this.Insert(this.Separator, item);
                this.Separator++;
            }
        }

        public void AddPost(T item)
        {
            this.Add(item);

            if (this.Separator < 0)
            {
                this.Separator = this.Count - 1;
            }
        }

        public readonly Span<T> AsSpan() =>
            this.items.AsSpan(0, this.Count);

        public void Clear()
        {
            this.items.AsSpan().Clear();

            this.Count     = 0;
            this.Separator = -1;
        }

        public void ForceCount(int count) =>
            this.Count = count;

        public void Insert(int index, T item)
        {
            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.EnsureCapacity();

            this.Count++;

            var length = this.Count - index - 1;

            if (length > 0)
            {
                var source      = this.items.AsSpan(index, length);
                var destination = this.items.AsSpan(index + 1, length);

                source.CopyTo(destination);
            }

            this.items[index] = item;

            if (index < this.Separator)
            {
                this.Separator++;
            }
        }

        public bool Remove(T item)
        {
            var index = this.items.IndexOf(item);

            if (index > -1)
            {
                this.RemoveAt(index);

                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var endIndex = index + 1;
            var length   = this.Count - endIndex;

            if (length > 0)
            {
                var source      = this.items.AsSpan(endIndex, length);
                var destination = this.items.AsSpan(index,    length);

                source.CopyTo(destination);
            }

            this.Count--;

            this.items[this.Count] = default!;

            if (index < this.Separator)
            {
                this.Separator--;
            }
        }
    }
}
