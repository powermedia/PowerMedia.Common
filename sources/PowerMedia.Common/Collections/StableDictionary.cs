using System;
using System.Collections.Generic;
using System.Text;

namespace PowerMedia.Common.Collections
{
    /// <summary>
    /// Dictionary with preserved order of insertion
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class StableDictionary<Key, Value> : Dictionary<Key, Value>, IEnumerable<Key>
    {
        private List<Key> _stableKeyCollection = new List<Key>();
        private List<Value> _stableValueCollection = new List<Value>();

        /// <summary>
        /// Add new dictionary item.
        /// Complexity O(1) (amortised).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(Key key, Value value)
        {
            base.Add(key, value);
            _stableKeyCollection.Add(key);
            _stableValueCollection.Add(value);
        }
        /// <summary>
        /// Removes item with given key.
        /// Complexity O(n).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new bool Remove(Key key)
        {
            return _stableKeyCollection.Remove(key) && _stableValueCollection.Remove(base[key]) && base.Remove(key);
        }

        public Value GetElementByInsertionOrder(int position)
        {
            if (position < 0 || position >= _stableKeyCollection.Count)
            {
                throw new IndexOutOfRangeException();
            }
            return base[_stableKeyCollection[position]];
        }

        /// <summary>
        /// Return collection of added keys with preserved order of insertion
        /// </summary>
        public new ICollection<Key> Keys
        {
            get
            {
                return _stableKeyCollection;
            }
        }
        /// <summary>
        /// Return collection of values keys with preserved order of insertion
        /// </summary>
        public new ICollection<Value> Values
        {
            get
            {
                return _stableValueCollection;
            }
        }

        #region IEnumerable<Key> Members

        public new IEnumerator<Key> GetEnumerator()
        {
            foreach (Key key in _stableKeyCollection)
            {
                yield return key;
            }
        }

        #endregion
    }
}
