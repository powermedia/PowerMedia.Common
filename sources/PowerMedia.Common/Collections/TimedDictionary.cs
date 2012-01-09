using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerMedia.Common.Collections
{
    [Serializable]
    public class TimedDictionary<KeyType, ElementType> : IDictionary<KeyType, ElementType>
    {

        public TimeSpan ElementExpirationTime { get; private set; }

        private readonly Dictionary<KeyType, ElementType> _storage = new Dictionary<KeyType, ElementType>();
        private readonly Dictionary<KeyType, DateTime> _expirationTimes = new Dictionary<KeyType, DateTime>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementExpiractionTime">in miliseconds</param>
        public TimedDictionary(TimeSpan elementExpiractionTime)
        {
            ElementExpirationTime = elementExpiractionTime;
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<KeyType, ElementType>> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(KeyValuePair<KeyType, ElementType> item)
        {
            this[item.Key] = item.Value;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            _storage.Clear();
            _expirationTimes.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(KeyValuePair<KeyType, ElementType> item)
        {
            return ((ICollection<KeyValuePair<KeyType, ElementType>>) _storage).Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(KeyValuePair<KeyType, ElementType>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<KeyType, ElementType>>) _storage).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(KeyValuePair<KeyType, ElementType> item)
        {

            var isRemoved = ((ICollection<KeyValuePair<KeyType, ElementType>>)_storage).Remove(item);
            if(!isRemoved)
            {
                return false;
            }
            var isRemovedFromTimes = _expirationTimes.Remove(item.Key);
            if( ! isRemovedFromTimes )
            {
                throw new InvalidOperationException();
            }
            return true;
        }

        /// <summary>
        /// Gets the number of all elements even are expired.
        /// </summary>
        public int Count
        {
            get
            {
                if( _storage.Count != _expirationTimes.Count )
                {
                    throw new InvalidOperationException();
                }
                return _storage.Count;
            }
        }

        /// <summary>
        /// Gets the number of all elements are not expired.
        /// </summary>
        public int CountNotExpired
        {
            get
            {
                if( _storage.Count != _expirationTimes.Count )
                {
                    throw new InvalidOperationException();
                }
                return _storage.Count(x => _expirationTimes[x.Key] < DateTime.Now);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an Element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an Element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(KeyType key)
        {
            return _storage.ContainsKey(key) && _expirationTimes.ContainsKey(key);
        }

        /// <summary>
        /// Adds an Element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the Element to add.</param><param name="value">The object to use as the value of the Element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An Element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public void Add(KeyType key, ElementType value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Removes the Element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the Element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the Element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public bool Remove(KeyType key)
        {
            if( ContainsKey(key) == false )
            {
                return false;
            }
            bool result = true;
            result &= _storage.Remove(key);
            result &= _expirationTimes.Remove(key);
            return result;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an Element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool TryGetValue(KeyType key, out ElementType value)
        {
            if( ContainsKey(key) )
            {
                value = this[key];
                return true;
            }
            value = default(ElementType);
            return false;
        }

        /// <summary>
        /// Gets or sets the Element with the specified key.
        /// </summary>
        /// <returns>
        /// The Element with the specified key.
        /// </returns>
        /// <param name="key">The key of the Element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public ElementType this[KeyType key]
        {
            get
            {
                if ( key == null )
                { 
                    throw new ArgumentNullException(); 
                }
                if ( ContainsKey(key) == false )
                { 
                    return default(ElementType); 
                }
                if (_expirationTimes[key] < DateTime.Now) 
                { 
                    return default(ElementType); 
                }
                return _storage[key];
            }
            set
            {
                _storage[key] = value;
                _expirationTimes[key] = DateTime.Now + ElementExpirationTime;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<KeyType> Keys
        {
            get { return _storage.Keys; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the UNEXPIRED values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the UNEXPIRED values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// 
        /// TODO: add lazy loading
        public ICollection<ElementType> Values
        {
            get
            {
                var now = DateTime.Now;
                var unExpired = from element in _storage
                                from time in _expirationTimes
                                where element.Key.Equals(time.Key) &&
                                      time.Value > now
                                select element.Value;
                return unExpired.ToList();
            }
        }

        /// <summary>
        /// Returns the dictionary elements which time has expired
        /// </summary>
        /// <returns></returns>
        public ICollection<KeyValuePair<KeyType, ElementType>> GetExpiredElements()
        {
            var now = DateTime.Now;
            var expired = from element in _storage
                          from time in _expirationTimes
                          where element.Key.Equals(time.Key) &&
                                time.Value < now
                          select element;
            return expired.ToList();
        }

        /// <summary>
        /// Removes expired elements from the dictionary which time has expired
        /// </summary>
        public void ClearExpiredElements()
        {
            var expired = GetExpiredElements();
            foreach (var element in expired)
            {
                Remove(element);
            }
        }

        public int SecondsToExpire(KeyType key)
        {
            if (ContainsKey(key))
            {
                var timespan = new TimeSpan(_expirationTimes[key].Ticks - DateTime.Now.Ticks);
                return Convert.ToInt32(timespan.TotalSeconds);
            }
            return default(int);
        }
    }
}
