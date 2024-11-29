using System.Runtime.CompilerServices;
using System.Buffers;

namespace CustomCollections
{
    public class CustomHashTable<TKey, TValue>
    {
        private struct Entry
        {
            public readonly uint HashCode;
            public readonly TKey Key;
            public TValue Value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Entry(uint hashCode, TKey key, TValue value)
            {
                HashCode = hashCode;
                Key = key;
                Value = value;
            }
        }

        private Entry[] _entries;
        private int _size;
        private int _capacity;
        private const float LoadFactor = 0.75f;
        private readonly ArrayPool<Entry> _pool = ArrayPool<Entry>.Shared;

        public CustomHashTable(int initialCapacity = 16)
        {
            _capacity = initialCapacity;
            _entries = _pool.Rent(_capacity);
            _size = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            uint hashCode = GetInternalHashCode(key);
            int index = FindIndex(key, hashCode);

            if (index >= 0)
            {
                value = _entries[index].Value;
                return true;
            }

            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key, TValue value)
        {
            if (_size >= _capacity * LoadFactor)
            {
                Resize();
            }

            uint hashCode = GetInternalHashCode(key);
            int index = FindInsertIndex(key, hashCode);

            _entries[index] = new Entry(hashCode, key, value);
            _size++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint GetInternalHashCode(TKey key)
        {
            return (uint)(key?.GetHashCode() & 0x7FFFFFFF ?? 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindIndex(TKey key, uint hashCode)
        {
            int index = (int)(hashCode & (_capacity - 1));
            int i = 1;

            // Quadratic probing to resolve collisions
            while (_entries[index].HashCode != 0)
            {
                if (_entries[index].HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(_entries[index].Key, key))
                {
                    return index;
                }

                index = (index + i * i) & (_capacity - 1);  // Quadratic probing with bitwise AND
                i++;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindInsertIndex(TKey key, uint hashCode)
        {
            int index = (int)(hashCode & (_capacity - 1));  // Faster bitwise AND instead of modulus
            int i = 1;

            // Quadratic probing to resolve collisions
            while (_entries[index].HashCode != 0)
            {
                if (_entries[index].HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(_entries[index].Key, key))
                {
                    throw new ArgumentException("Key already exists in the hash table.");
                }

                index = (index + i * i) & (_capacity - 1);  // Quadratic probing with bitwise AND
                i++;
            }

            return index;
        }

        private void Resize()
        {
            int newCapacity = _capacity * 2;  // Increase capacity by 100% (power of 2)
            Entry[] newEntries = _pool.Rent(newCapacity);

            for (int i = 0; i < _entries.Length; i++)
            {
                if (_entries[i].HashCode != 0)
                {
                    int newIndex = (int)(_entries[i].HashCode & (newCapacity - 1));  // Faster bitwise AND
                    int j = 1;

                    // Quadratic probing to resolve collisions during resize
                    while (newEntries[newIndex].HashCode != 0)
                    {
                        newIndex = (newIndex + j * j) & (newCapacity - 1);  // Quadratic probing with bitwise AND
                        j++;
                    }

                    newEntries[newIndex] = _entries[i];
                }
            }

            ClearArray(_entries);
            _pool.Return(_entries);
            _entries = newEntries;
            _capacity = newCapacity;
        }

        private void ClearArray(Entry[] array)
        {
            Array.Clear(array, 0, array.Length);
        }

        [System.Runtime.CompilerServices.MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref Entry FindEntry(int index)
        {
            return ref _entries[index];  // Use ref to avoid bounds checking multiple times
        }
    }
}
