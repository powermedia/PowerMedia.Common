using System;
using System.Collections.Generic;
using System.Text;

namespace PowerMedia.Common.Collections
{
    public class PairComparer<T, K> : IComparer<Pair<T,K>> where K : IComparable
    {

        public int Compare(Pair<T, K> x, Pair<T, K> y)
        {
            return x.Right.CompareTo(y.Right);
        }

    }
    [Serializable]
    public class Pair<T,K>
    {
        public Pair(T a, K b)
        {
            _left = a;
            _right = b;
        }
        private T _left;
        private K _right;

        public T Left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
            }
        }

        public K Right
        {
            get
            {
                return _right;
            }
            set
            {
                _right = value;
            }
        }
    }
}
