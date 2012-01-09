using System;
using NUnit.Framework;
using PowerMedia.Common;
using System.Collections.Generic;
using PowerMedia.Common.Collections;


namespace PowerMedia.Common.Tests
{
	[TestFixture]
	public class RandomUtilsTests
	{	
		[Test]
		public void GetRandomLetterFromPolishAlphabet_Test()
		{
			var randomChar = RandomUtils.GetRandomLetterFromPolishAlphabet();
			CollectionAssert.Contains(RandomUtils.PolishAlphabetLetters, randomChar);		
		}
		
		[Test]
		public void RandomByteArray_Test()
		{
			Assert.AreEqual(10, RandomUtils.RandomByteArray(10).Length);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomByteArray_WrongSize()
		{
			byte[] randomByteArray = RandomUtils.RandomByteArray(-10);
		}
		
		[Test]
		public void RandomIntArray_Test()
		{
			Assert.AreEqual(10, RandomUtils.RandomIntArray(10).Length);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomIntArray_WithUpperbound_WrongSize()
		{
			int[] randomArray = RandomUtils.RandomIntArray(-10, 19);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomIntArray_WithUpperbound_WrongUpperbound()
		{
			int[] randomArray = RandomUtils.RandomIntArray(10, -9);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomIntArray_WithUpperboundAndLowerbound_WrongSize()
		{
			int[] tab = RandomUtils.RandomIntArray(-2, 10, 13);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomIntArray_WithUpperboundAndLowerbound_LowerboundGreaterThanUpperbound()
		{
			int[] tab = RandomUtils.RandomIntArray(10, 12, 3);
		}
		
		[Test]
		public void RandomIntArray_WithUpperboundAndLowerbound_Test()
		{
			int[] randomArray = RandomUtils.RandomIntArray(100, 100, 1000);
			Assert.AreEqual(100, randomArray.Length);
			foreach(int element in randomArray)
			{
				Assert.GreaterOrEqual(element, 100);
				Assert.LessOrEqual(element, 1000);
			}
		}
		
		[Test]
		public void RandomIntArray_WithUpperbound_Test()
		{
			int[] randomArray = RandomUtils.RandomIntArray(10, 1000);
			Assert.AreEqual(10, randomArray.Length);
			foreach(int element in randomArray)
			{
				Assert.LessOrEqual(element, 1000);
			}
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomIntArray_WrongSize()
		{
			int[] randomArray = RandomUtils.RandomIntArray(-10);
		}
		
		[Test]
		public void GetRandomPolishCharList_SizeTest()
		{
			Assert.AreEqual(10, RandomUtils.GetRandomPolishCharList(10).Count);
		}
		
		[Test]
		public void GetRandomPolishCharList_DifferentCharsTest()
		{
			List<char> randomCharList = RandomUtils.GetRandomPolishCharList(25);			
			CollectionAssert.AllItemsAreUnique(randomCharList);
		}
		
		[Test]
		public void RandomDoubleArray_SizeTest()
		{
			Assert.AreEqual(10, RandomUtils.RandomDoubleArray(10).Length);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomDoubleArray_WrongArgument()
		{
			double[] randomArray = RandomUtils.RandomDoubleArray(-10);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomDoubleArray_WithUpperbound_WrongSize()
		{
			double[] randomArray = RandomUtils.RandomDoubleArray(-10, 19);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomDoubleArray_WithUpperbound_WrongUpperbound()
		{
			double[] randomArray = RandomUtils.RandomDoubleArray(10, -9);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomDoubleArray_WithUpperboundAndLowerbound_WrongSize()
		{
			double[] randomArray = RandomUtils.RandomDoubleArray(-2, 10, 13);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomDoubleArray_WithUpperboundAndLowerbound_WrongRange()
		{
			double[] randomArray = RandomUtils.RandomDoubleArray(10, 12, 3);
		}
		
		[Test]
		public void RandomDoubleArray_WithUpperboundAndLowerbound_Test()
		{
			double[] randomArray = RandomUtils.RandomDoubleArray(100, 100, 1000);
			Assert.AreEqual(100, randomArray.Length);
			foreach(double element in randomArray)
			{
				Assert.GreaterOrEqual(element, 100);
				Assert.LessOrEqual(element, 1000);
			}
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomDate_FromRange_RangeStartGreaterThanRangeEnd()
		{
			RandomUtils.RandomDate(
				new DateTime(2999, 01, 01),
				new DateTime(2003, 03, 01)
			);
		}
		
		[Test]
		public void RandomDoubleArray_WithUpperbound_Test()
		{
			double[] randomArray = RandomUtils.RandomDoubleArray(10,1000);
			Assert.AreEqual(10, randomArray.Length);
			foreach(double element in randomArray)
			{
				Assert.LessOrEqual(element, 1000);
			}
		}
		
		[Test]
		public void RandomString_WithLettersFromRange_Test()
		{
			string randomString = RandomUtils.RandomString(20, 'b', 'p');
			Assert.AreEqual(20, randomString.Length);
			foreach(char character in randomString)
			{
				Assert.GreaterOrEqual(character, 'b');
				Assert.LessOrEqual(character, 'p');
			}
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomString_WrongSize()
		{
			Assert.AreEqual(20, RandomUtils.RandomString(-20,'b','p').Length);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomPolishCharactersString_WrongSize()
		{
			Assert.AreEqual(20, RandomUtils.RandomPolishCharactersString(-20).Length);
		}
		
		[Test]
		public void RandomPolishCharacters_String()
		{
			string randomString = RandomUtils.RandomPolishCharactersString(20);
			Assert.AreEqual(20, randomString.Length);
		}
		
		[Test]
		public void RandomLong_Test()
		{
			Assert.IsNotNull(RandomUtils.RandomLong());
		}
		
		[Test]
		public void RandomLong_FromRange_Test()
		{
			var lowerBound     = long.MaxValue * (1/2);
			var upperBound	   = long.MaxValue * (3/4);
			long randomLong = RandomUtils.RandomLong
			(
				lowerBound,
				upperBound
			);
			Assert.GreaterOrEqual(randomLong, lowerBound);
			Assert.LessOrEqual   (randomLong, upperBound);
		}
		
		[Test]
		public void RandomComplexString_Test()
		{
			Pair<int,RandomUtils.CharRange>[] coding =
			{
				new Pair<int,RandomUtils.CharRange>(5,RandomUtils.CharRange.ASCIIWithoutEscape),
				new Pair<int,RandomUtils.CharRange>(5,RandomUtils.CharRange.Escape),
				new Pair<int,RandomUtils.CharRange>(5,RandomUtils.CharRange.HigherCodes)
			};			
			string randomString = RandomUtils.RandomComplexString(coding);
			Assert.IsNotNull(randomString);
		}
		
		[Test]
		public void RandomComplexString_CorrectnessChars_Test()
		{
			Pair<int,RandomUtils.CharRange>[] coding =
			{
				new Pair<int,RandomUtils.CharRange>(5,RandomUtils.CharRange.ASCIIWithoutEscape),
				new Pair<int,RandomUtils.CharRange>(5,RandomUtils.CharRange.Escape),
				new Pair<int,RandomUtils.CharRange>(5,RandomUtils.CharRange.HigherCodes),
				new Pair<int,RandomUtils.CharRange>(5,RandomUtils.CharRange.PL)
			};			
			string randomString = RandomUtils.RandomComplexString(coding);
			char[] charactersFromRandomString = randomString.ToCharArray();
			Pair<char, char> charsRange;
			for(int index=0; index<=4; index++)
			{
				RandomUtils.CharactersRange_Of_EncodingScheme.TryGetValue
					(
						RandomUtils.CharRange.ASCIIWithoutEscape,
						out charsRange
					);
				Assert.GreaterOrEqual(charactersFromRandomString[index], charsRange.Left);
				Assert.LessOrEqual(charactersFromRandomString[index], charsRange.Right);
			}
			for(int index=5; index<=9; index++)
			{
				RandomUtils.CharactersRange_Of_EncodingScheme.TryGetValue
					(
						RandomUtils.CharRange.Escape,
						out charsRange
					);
				Assert.GreaterOrEqual(charactersFromRandomString[index], charsRange.Left);
				Assert.LessOrEqual(charactersFromRandomString[index], charsRange.Right);
			}
			for(int index=10; index<=14; index++)
			{
				RandomUtils.CharactersRange_Of_EncodingScheme.TryGetValue
					(
						RandomUtils.CharRange.HigherCodes,
						out charsRange
					);
				Assert.GreaterOrEqual(charactersFromRandomString[index], charsRange.Left);
				Assert.LessOrEqual(charactersFromRandomString[index], charsRange.Right);
			}
			for(int index=15; index<=19; index++)
			{
				CollectionAssert.Contains
					(
						RandomUtils.PolishCharacters,
						charactersFromRandomString[index]
					);
			}
		}
			
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RandomComplexString_NullArgument()
		{
			string randomString = RandomUtils.RandomComplexString(null);
		}
		
		[Test]
		public void RandomDate_Test()
		{
			Assert.NotNull(RandomUtils.RandomDate());
		}
		
		[Test]
		public void RandomSelect_Test()
		{
			List<double> testList = new List<double>()
			{
			    4.3, 2.3, 3.3, 4.4, 4.1
			};
			CollectionAssert.Contains(testList, RandomUtils.RandomSelect(testList));
		}
		
		[Test]
		public void RandomArray_Test()
		{
			List<double> testList = new List<double>()
			{
			    4.3, 2.3, 3.3, 4.4, 4.1, 4.1
			};
			double[] testArray = RandomUtils.RandomArray(testList, 3);
			Assert.AreEqual(3,testArray.Length);
			foreach(double element in testArray)
			{
				CollectionAssert.Contains(testList,element);
			}
			
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RandomArray_WrongSize()
		{
			RandomUtils.RandomArray(new List<int>(), -10);
		}
		
	}
}
