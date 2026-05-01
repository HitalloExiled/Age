using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe partial struct NativeDictionary<K, V> where K : unmanaged, IEquatable<K>
where V : unmanaged
{
    [DebuggerTypeProxy(typeof(NativeDictionary<,>.KeyValueCollection.DebugView))]
    public readonly partial struct KeyValueCollection
    {
        private readonly NativeDictionary<K, V> dictionary;

        public KeyValueCollection(NativeDictionary<K, V> dictionary)
        {
            if (!dictionary.IsCreated)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            this.dictionary = dictionary;
        }

        public readonly int Count => this.dictionary.Count;

        public readonly UnsafeDictionary.Enumerator<K, V> GetEnumerator() =>
            new(this.dictionary.inner);

        public readonly void CopyTo(Span<KeyValuePair<K, V>> array, int index)
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
                array[i++] = enumerator.Current;
            }
        }

        public readonly NativeArray<KeyValuePair<K, V>> ToNativeArray()
        {
            var values = new NativeArray<KeyValuePair<K, V>>(this.Count);

            this.CopyTo(values, 0);

            return values;
        }

        public readonly KeyValuePair<K, V>[] ToArray()
        {
            var values = new KeyValuePair<K, V>[this.Count];

            this.CopyTo(values, 0);

            return values;
        }

        public override string ToString() =>
            $"Count = {this.Count}";
    }
}
