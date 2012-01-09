using System;
using System.Runtime;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace PowerMedia.Common.Collections
{
    [global::System.Serializable]
    public class ReadOnlyViolationException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ReadOnlyViolationException() { }
        public ReadOnlyViolationException(string message) : base(message) { }
        public ReadOnlyViolationException(string message, Exception inner) : base(message, inner) { }
        protected ReadOnlyViolationException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
    //need implementation of containsvalue
    public class ImmutableDictionary<TKey, TValue> : IDictionary<TKey, TValue> // where TValue : class
    {
        private readonly IDictionary<TKey, TValue> _internalDictionary;
        private const string MSGREADONLY = "Trying to access a readonly value!";

        public ImmutableDictionary(IDictionary<TKey, TValue> dict)
        {
            _internalDictionary = dict;
        }

        public void Add(TKey key, TValue value)
        {
            throw new ReadOnlyViolationException();
        }

        public bool ContainsKey(TKey key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _internalDictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            throw new ReadOnlyViolationException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _internalDictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _internalDictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _internalDictionary[key];
            }
            set
            {
                throw new ReadOnlyViolationException();
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new ReadOnlyViolationException();
        }

        public void Clear()
        {
            throw new ReadOnlyViolationException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _internalDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _internalDictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _internalDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new ReadOnlyViolationException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    //@TODO: whole functionality of ArrayList, ReadOnly property
    //TODO need implements IList interface
    public class ImmutableArrayList
    {
        private readonly ArrayList _InternalArrayList;
        private const string MSGREADONLY = "Trying to access a readonly value!";

        public ImmutableArrayList(ArrayList arr)
        {
            _InternalArrayList = arr;            
        }

        public int Count
        {
            get { return _InternalArrayList.Count; }
        }

        public object this[int index]
        {
            get { return _InternalArrayList[index]; }
            set { throw new ReadOnlyViolationException(MSGREADONLY); }
        }
    }
}
