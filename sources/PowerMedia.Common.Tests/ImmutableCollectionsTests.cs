using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using PowerMedia.Common.Collections;

namespace PowerMedia.Common.Tests.Collections
{
    [TestFixture]
    [Timeout(100)]
    public class ImmutableCollectionTests
    {
        [Test]
        public void ImmutableDictionary_Test()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                {1,"asaas"},
                {2,"sasas"},
                {3,"tak"}
            };
            ImmutableDictionary<int, string> test = new ImmutableDictionary<int, string>(dictionary);

            Assert.AreEqual(test.Count, dictionary.Count);

            foreach (var key in dictionary.Keys)
			{
				Assert.AreEqual(dictionary[key], test[key]);
			}
            Assert.IsTrue(test.IsReadOnly);
            Assert.AreEqual(test[dictionary.Keys.First()], dictionary.Values.First() );
            Assert.AreEqual(dictionary.Keys, test.Keys);
            Assert.AreEqual(dictionary.Values, test.Values);
        }

        [Test]
        [ExpectedException(typeof(ReadOnlyViolationException))]
        public void ImmutableDictionary_AddTest()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                {1,"asaas"},
                {2,"sasas"},
                {3,"tak"}
            };
            ImmutableDictionary<int, string> test = new ImmutableDictionary<int, string>(dictionary);
            test.Add(3, "dddfd");
        }

        [Test]
        [ExpectedException(typeof(ReadOnlyViolationException))]
        public void ImmutableDictionary_RemoveTest()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                {1,"asaas"},
                {2,"sasas"},
                {3,"tak"}
            };
            ImmutableDictionary<int, string> test = new ImmutableDictionary<int, string>(dictionary);
            test.Remove(2);
        }

        [Test]
        [ExpectedException(typeof(ReadOnlyViolationException))]
        public void ImmutableDictionary_ClearTest()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                {1,"asaas"},
                {2,"sasas"},
                {3,"tak"}
            };
            ImmutableDictionary<int, string> test = new ImmutableDictionary<int, string>(dictionary);
            test.Clear();
        }

        [Test]
        [ExpectedException(typeof(ReadOnlyViolationException))]
        public void ImmutableDictionary_ModifyTest()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                {1,"asaas"},
                {2,"sasas"},
                {3,"tak"}
            };
            ImmutableDictionary<int, string> test = new ImmutableDictionary<int, string>(dictionary);
            test[2]="a";
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ImmutableDictionary_OutOfRangeTest()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                {1,"asaas"},
                {2,"sasas"},
                {3,"tak"}
            };
            ImmutableDictionary<int, string> test = new ImmutableDictionary<int, string>(dictionary);
            string t = test[15];
        }

        [Test]
        public void ImmutableArrayList_Test()
        {
            ArrayList arrayList = new ArrayList() {1,"tak",34m, 15.01 };
            ImmutableArrayList test = new ImmutableArrayList(arrayList);
            Assert.AreEqual(4, test.Count);
            Assert.AreEqual("tak", test[1]);
        }

        [Test]
        [ExpectedException(typeof(ReadOnlyViolationException))]
        public void ImmutableArrayList_ModifyTest()
        {
            ArrayList arrayList = new ArrayList() { 1, "tak", 34m, 15.01 };
            ImmutableArrayList test = new ImmutableArrayList(arrayList);
            test[2] = 231312;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ImmutableArrayList_OutOfRangeTest()
        {
            ArrayList arrayList = new ArrayList() { 1, "tak", 34m, 15.01 };
            ImmutableArrayList test = new ImmutableArrayList(arrayList);
            test[15].GetType();
        }
        
        [Test]
        public void ContainsKey_Test()
        {
        	Dictionary<int, string> _dictionary = new Dictionary<int, string>
            {
                {1, "aabb"},
                {2, "bbcc"},
                {3, "ccdd"}
            };
        	ImmutableDictionary<int, string> _immutableDictionary = new ImmutableDictionary<int, string>(_dictionary);        	
        	Assert.IsTrue(_immutableDictionary.ContainsKey(1));
        }
        
        [Test]
        [ExpectedException(typeof(ReadOnlyViolationException))]
        public void Add_Test()
        {
        	Dictionary<int, string> _dictionary = new Dictionary<int, string>
            {
                {1, "aabb"},
                {2, "bbcc"},
                {3, "ccdd"}
            };
        	ImmutableDictionary<int, string> _immutableDictionary = new ImmutableDictionary<int, string>(_dictionary);
        	_immutableDictionary.Remove(new KeyValuePair<int, string>(1, "aabb"));
        }
        
        [Test]
        [ExpectedException(typeof(ReadOnlyViolationException))]
        public void Remove_Test()
        {
        	Dictionary<int, string> _dictionary = new Dictionary<int, string>
            {
                {1, "aabb"},
                {2, "bbcc"},
                {3, "ccdd"}
            };
        	ImmutableDictionary<int, string> _immutableDictionary = new ImmutableDictionary<int, string>(_dictionary);
        	_immutableDictionary.Add(new KeyValuePair<int, string>(4, "ddee"));
        }
        
        [Test]
        public void Contains_Test()
        {
        	Dictionary<int, string> _dictionary = new Dictionary<int, string>
            {
                {1, "aabb"},
                {2, "bbcc"},
                {3, "ccdd"}
            };
        	ImmutableDictionary<int, string> _immutableDictionary = new ImmutableDictionary<int, string>(_dictionary);
        	Assert.IsTrue(_immutableDictionary.Contains(new KeyValuePair<int, string>(1, "aabb")));
        	Assert.IsFalse(_immutableDictionary.Contains(new KeyValuePair<int, string>(1, "aabbcc")));
        }
    }
}
