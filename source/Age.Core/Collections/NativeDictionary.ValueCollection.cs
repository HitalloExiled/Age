using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe partial struct NativeDictionary<K, V> where K : unmanaged, IEquatable<K>
where V : unmanaged
{
    [DebuggerTypeProxy(typeof(NativeDictionary<,>.ValueCollection.DebugView))]
    public readonly partial struct ValueCollection
    {
        private readonly NativeDictionary<K, V> dictionary;

        public ValueCollection(NativeDictionary<K, V> dictionary)
        {
            if (!dictionary.IsCreated)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            this.dictionary = dictionary;
        }

        public readonly int Count => this.dictionary.Count;

        public readonly UnsafeDictionary.ValueEnumerator<V> GetEnumerator() =>
            new(this.dictionary.inner);

        public readonly void CopyTo(Span<V> array, int index)
        {
            if ((uint)index > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Length - index < this.Count)
            {
                throw new ArgumentException("Insufficient space in the target location to copy the information.");
            }

            if (array.Length == 0)
            {
                return;
            }

            var i = index;

            var enumerator = this.dictionary.GetEnumerator();

            while (enumerator.MoveNext())
            {
                array[i++] = enumerator.CurrentValue;
            }
        }

        public readonly NativeArray<V> ToNativeArray()
        {
            var values = new NativeArray<V>(this.Count);

            this.CopyTo(values, 0);

            return values;
        }

        public readonly V[] ToArray()
        {
            var values = new V[this.Count];

            this.CopyTo(values, 0);

            return values;
        }

        public override string ToString() =>
            $"Count = {this.Count}";
    }
}
