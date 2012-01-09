using System;
using NUnit.Framework;
using PowerMedia.Common.Collections;

namespace PowerMedia.Common.Tests.Collections
{
	[TestFixture]
	public class StableDictionaryTests
	{
		[Test]
		public void InitializeTest()
		{
			StableDictionary<int, string> dictionary = new StableDictionary<int, string>();
			Assert.IsNotNull(dictionary);
			Assert.AreEqual(0, dictionary.Values.Count);
			Assert.AreEqual(0, dictionary.Keys.Count);
		}
		
		[Test]
		public void StableDictionary_AddElement_Test()
		{
			StableDictionary<int, string> dictionary = new StableDictionary<int, string>();
			dictionary.Add(1, "a");
			dictionary.Add(2, "b");
			dictionary.Add(3, "c");
			dictionary.Add(4, "d");
			Assert.AreEqual(4, dictionary.Count);
			Assert.IsTrue(dictionary.ContainsKey(1));
			Assert.IsTrue(dictionary.ContainsKey(2));
			Assert.IsTrue(dictionary.ContainsKey(3));
			Assert.IsTrue(dictionary.ContainsKey(4));
			Assert.IsTrue(dictionary.ContainsValue("a"));
			Assert.IsTrue(dictionary.ContainsValue("b"));
			Assert.IsTrue(dictionary.ContainsValue("c"));
			Assert.IsTrue(dictionary.ContainsValue("d"));
		}
		
		[Test]
		public void StableDictionary_RemoveElement_Test()
		{
			StableDictionary<int, string> dictionary = new StableDictionary<int, string>();
			dictionary.Add(1, "a");
			dictionary.Add(2, "b");
			dictionary.Add(3, "c");
			dictionary.Add(4, "d");
			dictionary.Remove(1);
			Assert.AreEqual(3, dictionary.Count);
			Assert.IsFalse(dictionary.ContainsKey(1));
			Assert.IsFalse(dictionary.ContainsValue("a"));
		}
		
		[Test]
		public void GetElementByInsertionOrder_Test()
		{
			StableDictionary<int, string> dictionary = new StableDictionary<int, string>();
			dictionary.Add(4, "a");
			dictionary.Add(3, "b");
			dictionary.Add(2, "c");
			dictionary.Add(1, "d");	
			Assert.AreEqual("d", dictionary.GetElementByInsertionOrder(3));
		}
		
		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void GetElementByInsertionOrder_WrongPosition()
		{
			StableDictionary<int, string> dictionary = new StableDictionary<int, string>();
			dictionary.GetElementByInsertionOrder(-1);
		}
				
			
	}
}
