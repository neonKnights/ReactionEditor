using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pkg.generic_serializable_dictionary.Scripts
{
    /// <summary>
    /// Generic Serializable Dictionary for Unity 2020.1.
    /// Simply declare your key/value types and you're good to go - zero boilerplate.
    /// </summary>
    [Serializable]
    public class GenericDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        // Internal
        [SerializeField]
        List<KeyValuePair> list = new List<KeyValuePair>();
        [SerializeField]
        Dictionary<TKey, int> indexByKey = new Dictionary<TKey, int>();
        [SerializeField, HideInInspector]
        Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

#pragma warning disable 0414
        [SerializeField, HideInInspector]
        bool keyCollision;
#pragma warning restore 0414

        // Serializable KeyValuePair struct
        [Serializable]
        struct KeyValuePair
        {
            public TKey Key;
            public TValue Value;

            public KeyValuePair(TKey Key, TValue Value)
            {
                this.Key = Key;
                this.Value = Value;
            }
        }

        // Since lists can be serialized natively by unity no custom implementation is needed
        public void OnBeforeSerialize() { }

        // Fill dictionary with list pairs and flag key-collisions.
        public void OnAfterDeserialize()
        {
            this.dict.Clear();
            this.indexByKey.Clear();
            this.keyCollision = false;

            for (int i = 0; i < this.list.Count; i++)
            {
                var key = this.list[i].Key;
                if (key != null && !this.ContainsKey(key))
                {
                    this.dict.Add(key, this.list[i].Value);
                    this.indexByKey.Add(key, i);
                }
                else
                {
                    this.keyCollision = true;
                }
            }
        }

        // IDictionary
        public TValue this[TKey key]
        {
            get => this.dict[key];
            set
            {
                this.dict[key] = value;

                if (this.indexByKey.ContainsKey(key))
                {
                    var index = this.indexByKey[key];
                    this.list[index] = new KeyValuePair(key, value);
                }
                else
                {
                    this.list.Add(new KeyValuePair(key, value));
                    this.indexByKey.Add(key, this.list.Count - 1);
                }
            }
        }

        public ICollection<TKey> Keys => this.dict.Keys;
        public ICollection<TValue> Values => this.dict.Values;

        public void Add(TKey key, TValue value)
        {
            this.dict.Add(key, value);
            this.list.Add(new KeyValuePair(key, value));
            this.indexByKey.Add(key, this.list.Count - 1);
        }

        public bool ContainsKey(TKey key) => this.dict.ContainsKey(key);

        public bool Remove(TKey key) 
        {
            if (this.dict.Remove(key))
            {
                var index = this.indexByKey[key];
                this.list.RemoveAt(index);
                this.UpdateIndexes(index);
                this.indexByKey.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        void UpdateIndexes(int removedIndex) {
            for (int i = removedIndex; i < this.list.Count; i++) {
                var key = this.list[i].Key;
                this.indexByKey[key]--;
            }
        }

        public bool TryGetValue(TKey key, out TValue value) => this.dict.TryGetValue(key, out value);

        // ICollection
        public int Count => this.dict.Count;
        public bool IsReadOnly { get; set; }

        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            this.Add(pair.Key, pair.Value);
        }

        public void Clear()
        {
            this.dict.Clear();
            this.list.Clear();
            this.indexByKey.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            TValue value;
            if (this.dict.TryGetValue(pair.Key, out value))
            {
                return EqualityComparer<TValue>.Default.Equals(value, pair.Value);
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (array.Length - arrayIndex < this.dict.Count)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            foreach (var pair in this.dict)
            {
                array[arrayIndex] = pair;
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            TValue value;
            if (this.dict.TryGetValue(pair.Key, out value))
            {
                bool valueMatch = EqualityComparer<TValue>.Default.Equals(value, pair.Value);
                if (valueMatch)
                {
                    return this.Remove(pair.Key);
                }
            }
            return false;
        }

        // IEnumerable
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.dict.GetEnumerator();
    }
}
