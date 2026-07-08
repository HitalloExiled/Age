using System.Diagnostics;

namespace Age.Core.Collections;

public unsafe partial struct NativeDictionary<K, V> where K : unmanaged, IEquatable<K>
where V : unmanaged
{
    [DebuggerTypeProxy(typeof(NativeDictionary<,>.KeyCollection.DebugView))]
    public readonly partial struct KeyCollection
    {
        private readonly NativeDictionary<K, V> dictionary;

        public KeyCollection(NativeDictionary<K, V> dictionary)
        {
            if (!dictionary.IsCreated)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            this.dictionary = dictionary;
        }

        public readonly int Count => this.dictionary.Count;

        public readonly UnsafeDictionary.KeyEnumerator<K> GetEnumerator() =>
            new(this.dictionary.inner);

        public readonly void CopyTo(Span<K> array, int index)
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
                array[i++] = enumerator.CurrentKey;
            }
        }

        public readonly NativeArray<K> ToNativeArray()
        {
            var keys = new NativeArray<K>(this.Count);

            this.CopyTo(keys, 0);

            return keys;
        }

        public readonly K[] ToArray()
        {
            var keys = new K[this.Count];

            this.CopyTo(keys, 0);

            return keys;
        }

        public override string ToString() =>
            $"Count = {this.Count}";
    }
}
