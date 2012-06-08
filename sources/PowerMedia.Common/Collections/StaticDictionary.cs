
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace PowerMedia.Common.Collections
{
    public class StaticDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary;

        public StaticDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this._dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public StaticDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this._dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public TValue this[TKey key]
        {
            get
            {
                return this._dictionary[key];
            }
            set
            {
                throw new NotSupportedException("This dictionary is readonly");
            }
        }
        
        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("This dictionary is readonly. Values can be added only in constructor.");
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("This dictionary is readonly. Values can be added only in constructor.");
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException("This dictionary is readonly");
        }

        public void Clear()
        {
            throw new NotSupportedException("This dictionary is readonly");
        }

        public bool ContainsKey(TKey key)
        {
            return this._dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get 
            {
                return this._dictionary.Keys;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get 
            {
                return this._dictionary.Values;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!this._dictionary.ContainsKey(item.Key))
                return false;

            return this._dictionary[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            //////////sratatatatat
        }

        public int Count
        {
            get 
            {
                return this._dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get 
            { 
                return true; 
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("This dictionary is readonly");
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this._dictionary.GetEnumerator();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return this._dictionary.GetEnumerator();
        }
    }
}
