using System.Collections;

namespace Age.Core.Collections;

public class DropoutStack<T>(int capacity) : IEnumerable<T>
{
    private readonly T[] items = new T[capacity];
    private int top;

    public int Count { get; private set; }

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    public T Pop()
    {
        this.top   = (capacity + this.top - 1) % capacity;
        this.Count = int.Max(this.Count - 1, 0);

        var item = this.items[this.top];

        this.items[this.top] = default!;

        return item;
    }

    public void Push(T item)
    {
        this.items[this.top] = item;

        this.top   = (this.top + 1) % capacity;
        this.Count = int.Min(this.Count + 1, capacity);
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (this.Count > 0)
        {
            if ((capacity + this.top - 1) % capacity == this.Count - 1)
            {
                for (var i = 0; i < this.Count; i++)
                {
                    yield return this.items[i];
                }
            }
            else
            {
                var start = (capacity + this.top - this.Count) % capacity;
                var end   = int.Min(start + this.Count, capacity);

                for (var i = start; i < end; i++)
                {
                    yield return this.items[i];
                }

                if (this.top < end)
                {
                    for (var i = 0; i < this.top; i++)
                    {
                        yield return this.items[i];
                    }
                }
            }
        }
    }
}
