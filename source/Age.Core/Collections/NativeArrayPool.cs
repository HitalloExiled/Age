namespace Age.Core.Collections;

public class NativeArrayPool<T> where T : unmanaged
{
    private readonly Lock @lock = new();
    private readonly Stack<NativeArray<T>> entries = [];

    public NativeArray<T> Get(int length)
    {
        lock (this.@lock)
        {
            if (this.entries.Count == 0)
            {
                return new NativeArray<T>(length);
            }
            else
            {
                var entry = this.entries.Pop();

                if (length != entry.Length)
                {
                    entry.Resize(length);
                }

                return entry;
            }
        }
    }

    public NativeArray<T> GetCopy(scoped ReadOnlySpan<T> span)
    {
        lock (this.@lock)
        {
            var array = this.Get(span.Length);

            span.CopyTo(array);

            return array;
        }
    }

    public void Return(NativeArray<T> item)
    {
        lock (this.@lock)
        {
            this.entries.Push(item);
        }
    }
}
