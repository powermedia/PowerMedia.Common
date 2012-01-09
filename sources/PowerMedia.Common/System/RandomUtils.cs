using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PowerMedia.Common.Collections;

namespace PowerMedia.Common
{
    public static class RandomUtils
    {
        public const string MS_SQLSERVER_SMALLDATE_MINIMUM = "1900-01-01 7:34:42Z";
        public const string MS_SQLSERVER_SMALLDATE_MAXIMUM = "2079-05-01 7:34:42Z";
		
        public static IList<char> PolishAlphabetLetters
        {
        	get
        	{
        		return new List<char>()
				{
					'A', 'a', 'Ą', 'ą', 'B', 'b', 'C', 'c', 'Ć', 'ć', 'D', 'd',
					'E', 'e', 'Ę', 'ę', 'F', 'f', 'G', 'g', 'H', 'h', 'I', 'i',
					'J', 'j', 'K', 'k', 'L', 'l', 'Ł', 'ł', 'M', 'm', 'N', 'n',
					'Ń', 'ń', 'O', 'o', 'Ó', 'ó', 'P', 'p', 'R', 'r', 'S', 's',
					'Ś', 'ś', 'T', 't', 'U', 'u', 'W', 'w', 'Y', 'y', 'Z', 'z',
					'Ź', 'ź', 'Ż', 'ż'
				};
        	}
        }
        
        public static char[] PolishCharacters
        {
        	get
        	{
        		return new char[]
	            {
	                'ą','Ą','Ę','ę','ć','Ć','ń','Ń','ł','Ł','ó','Ó','ś','Ś', (char)8222
	            }; 	
        	}
        }
        	
        //TODO: rename to EncodingScheme
        public enum CharRange { Escape, ASCIIWithoutEscape, PL, EuropeanMiddleEast, PrivateUse1, SymbolsAsian, Supplementary, PrivateUse2, HigherCodes }

        //TODO: check range of Escape and ASCIIWithoutEscape CharRanges, (char)33 is in both ranges
        private static readonly Dictionary<RandomUtils.CharRange, Pair<char, char>> 
        	CharactersRange_Of_EncodingScheme_OrdinaryDictionary
			= new Dictionary<RandomUtils.CharRange, Pair<char, char>>
			{
				{CharRange.Escape, new Pair<char, char>((char)0, (char)33)},
				{CharRange.ASCIIWithoutEscape, new Pair<char, char>((char)33, (char)127)},
				{CharRange.EuropeanMiddleEast, new Pair<char, char>((char)0x0080, (char)0x7FF)},
				{CharRange.PrivateUse1, new Pair<char, char>((char)0xE000, (char)0xF8FF)},
				{CharRange.PrivateUse2, new Pair<char, char>((char)0xDB80, (char)0xDBFF)},
				{CharRange.Supplementary, new Pair<char, char>((char)0xD800, (char)0xDB7F)},
				{CharRange.SymbolsAsian, new Pair<char, char>((char)0x0800, (char)0xD7FF)},
				{CharRange.HigherCodes, new Pair<char, char>((char)0xDC00, (char)0xDFFF)},
			};
        
        public static readonly ImmutableDictionary<RandomUtils.CharRange, Pair<char, char>>
        	CharactersRange_Of_EncodingScheme
        	= new ImmutableDictionary<RandomUtils.CharRange, Pair<char, char>>
        	(
        		CharactersRange_Of_EncodingScheme_OrdinaryDictionary
        	);
        
        private static Random _generator = new Random();

        //TODO: rename to GetUniqueRandomPolishCharList
        public static List<char> GetRandomPolishCharList(int length)
		{
			var result = new List<char>(length);
			for (int i = 0; i < length; ++i) {
				var randomChar = RandomUtils.GetRandomLetterFromPolishAlphabet();
				while( result.Contains(randomChar) )
				{
				      	randomChar = RandomUtils.GetRandomLetterFromPolishAlphabet();
				}
				result.Add(randomChar);
			}
			return result;
		}
        
        public static char GetRandomLetterFromPolishAlphabet()
		{
			var randomSource = new Random(DateTime.Now.Millisecond);
			Thread.Sleep(randomSource.Next(0,5));
			randomSource = new Random(DateTime.Now.Millisecond);
			var letter = PolishAlphabetLetters[ randomSource.Next(0, PolishAlphabetLetters.Count-1) ];
			char result = (char)letter;
			if( randomSource.Next() % 2 == 0)
			{
				result = char.ToLower((char)letter);
			}
			return result;
		}
        
        /// <summary>
        /// Method returning array of bytes with given size and random contents
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] RandomByteArray(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("size", size, "size must be positive");
            }
            byte[] result = new byte[size];
            _generator.NextBytes(result);
            return result;
        }

        /// <summary>
        /// Method returning array of integers with given size and random contents
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int[] RandomIntArray(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("size", size, "size must be positive");
            }
            int[] result = new int[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = _generator.Next();
            }
            return result;
        }

        /// <summary>
        /// Method returning array of integers with given size and random contents with given upper bound
        /// </summary>
        /// <param name="size"></param>
        /// <param name="lowerboundbound"></param>
        /// <param name="upperbound"></param>
        /// <returns></returns>
        public static int[] RandomIntArray(int size, int upperbound)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("size", size, "size must be positive");
            }
            if (upperbound < 0)
            {
                throw new ArgumentOutOfRangeException("upperbound", upperbound, "Upperbound can`t be negative");
            }
            int[] result = new int[size];
            
            for (int i = 0; i < size; i++)
            {
                result[i] = _generator.Next() % upperbound;
            }
            return result;
        }

        /// <summary>
        /// Method returning array of integers with given size and random contents with given lower and upper bound
        /// </summary>
        /// <param name="size"></param>
        /// <param name="lowerboundbound"></param>
        /// <param name="upperbound"></param>
        /// <returns></returns>
        public static int[] RandomIntArray(int size, int lowerbound, int upperbound)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("size", size, "size must be positive");
            }
            if (upperbound < lowerbound)
            {
                throw new ArgumentOutOfRangeException("upperbound", upperbound, "Upperbound can`t be lower than lowerbound");
            }
            int[] result = new int[size];
            int realUpperbound = upperbound - lowerbound;
            for (int i = 0; i < size; i++)
            {
                result[i] = _generator.Next() % realUpperbound + lowerbound;
            }
            return result;
        }


        /// <summary>
        /// Generate random array of doubles with given size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static double[] RandomDoubleArray(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("size", size, "size must be positive");
            }
            double[] result = new double[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = _generator.NextDouble();
            }
            return result;
        }

        /// <summary>
        /// Generate random array of doubles with given size and upperbound for generated numbers
        /// </summary>
        /// <param name="size"></param>
        /// <param name="upperbound"></param>
        /// <returns></returns>
        public static double[] RandomDoubleArray(int size, double upperbound)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("size", size, "size must be positive");
            }
            if (upperbound < 0)
            {
                throw new ArgumentOutOfRangeException("upperbound", upperbound, "Upperbound can`t be negative");
            }
            double[] result = new double[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = _generator.NextDouble()*upperbound;
            }
            return result;
        }

        /// <summary>
        /// Generate random array of doubles with given size, lower and upper bound for generated numbers
        /// </summary>
        /// <param name="size"></param>
        /// <param name="lowerbound"></param>
        /// <param name="upperbound"></param>
        /// <returns></returns>
        public static double[] RandomDoubleArray(int size, double lowerbound, double upperbound)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("size", size, "size must be positive");
            }
            if (upperbound < lowerbound)
            {
                throw new ArgumentOutOfRangeException("upperbound", upperbound, "Upperbound can`t be lower than lowerbound");
            }
            double[] result = new double[size];
            double realUpperbound = upperbound - lowerbound;
            for (int i = 0; i < size; i++)
            {
                result[i] = _generator.NextDouble() * realUpperbound+lowerbound;
            }
            return result;
        }

        /// <summary>
        /// Gets random
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kolekcja"></param>
        /// <returns></returns>
        public static T RandomSelect<T>(ICollection<T> aCollection) 
        {
            return aCollection.ElementAt<T>(_generator.Next(aCollection.Count));
        }

        public static T[] RandomArray<T>(ICollection<T> aCollection, int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("size", size, "size must be positive");
            }
            T[] result = new T[size];
            for(int i=0; i<size; i++)
            {
                result[i] = aCollection.ElementAt<T>(_generator.Next(aCollection.Count));
            }
            return result;
        }

        public static string RandomString(int length, char lowerbound, char upperbound)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length", length, "length must be positive");
            }

            if( upperbound < lowerbound )
            {
                var temp = upperbound;
                upperbound = lowerbound;
                lowerbound = temp;
            }

            char realUpperbound = (char)(upperbound - lowerbound+1);
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = (char)(_generator.Next() % realUpperbound + lowerbound);
            }
            return new string(result);
        }

        public static string RandomPolishCharactersString(int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length", length, "length must be positive");
            }           
            return new string(RandomArray((ICollection<char>)PolishCharacters, length));
        }

        public static string RandomComplexString(PowerMedia.Common.Collections.Pair<int,CharRange>[] coding)
        {
        	if(coding == null) 
        		throw new ArgumentNullException("coding");
            StringBuilder result = new StringBuilder();
            foreach (PowerMedia.Common.Collections.Pair<int, CharRange> pair in coding)
            {
                switch (pair.Right)
                {
                    case CharRange.PL:
                        result.Append(RandomPolishCharactersString(pair.Left));
                        break;

                    case CharRange.ASCIIWithoutEscape:
                    case CharRange.Escape:
                    case CharRange.PrivateUse1:
                    case CharRange.EuropeanMiddleEast:
                    case CharRange.PrivateUse2:
                    case CharRange.Supplementary:
                    case CharRange.SymbolsAsian:
                    case CharRange.HigherCodes:
						Pair<char, char> charsRange;
                        RandomUtils.CharactersRange_Of_EncodingScheme.TryGetValue
						(
							pair.Right,
							out charsRange
						);
                        result.Append(RandomString(pair.Left, charsRange.Left, charsRange.Right));
                        break;
                }
            }
            return result.ToString();
        }

        public static long RandomLong()
        {

            return (long)_generator.Next() + ((long)_generator.Next() << 32);
        }

        public static long RandomLong(long lowerbound, long upperbound)
        {
            if (upperbound < lowerbound)
            {
                throw new ArgumentOutOfRangeException("upperbound", upperbound, "Upperbound can`t be lower than lowerbound");
            }

            long realMax = upperbound - lowerbound;
            int intMax = (int)(realMax-(realMax>>32) >> 32);
            return lowerbound+((long)_generator.Next(intMax) + ((long)_generator.Next((int)(realMax >> 32))<<32));
        }

        public static DateTime RandomDate()
        {
            return new DateTime(RandomLong(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks));
        }

        public static DateTime RandomDate(DateTime minimum, DateTime maximum)
        {
            return new DateTime(RandomLong(minimum.Ticks, maximum.Ticks));
        }

        public static IList<string> ListOfDistinctPolischCharacterStrings(int count, int stringLength)
        {
            if (count < 1)
            {
                throw new ArgumentException("Argument value must not be less than 1.");
            }

            HashSet<string> results = new HashSet<string>();
            for (int k = 0; k < count; k++)
            {
                while (results.Add(RandomPolishCharactersString(stringLength)) == false) ;
            }

            return results.ToList<string>();
        }
    }
}
