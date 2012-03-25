using PowerMedia.Common.Collections;
using System;

namespace PowerMedia.Common.System
{
	public static class MathExtensions
	{
		public const int MAX_FACTORIAL_BASE = 12;
		
		public static int Factorial(this int number)
		{
			uint numberAsUint = (uint)number;
			return (int)(numberAsUint.Factorial());
		}

        private static uint[] factorialValues = new uint[] { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600 };
		public static uint Factorial(this uint number)
		{
			if( number < 0 )
			{
				throw new ArgumentOutOfRangeException();
			}
			
			if( number > MAX_FACTORIAL_BASE )
			{
				throw new ArgumentOutOfRangeException();
			}
			
			if( number == 0 )
			{
				return 1;
			}

            return factorialValues[number];
		}
		
		/// <summary>
		/// returns integer factorial for which is closest to the 'number'
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static uint ArcusFactorial(this uint number)
		{
			if( number <= 0)
			{
				throw new ArithmeticException();
			}
			if( number == 1 )
			{
				return 1;
			}
			var boundaries = ArcusFactorialBoundaries(number);
			var leftBoundary = Factorial(boundaries.Left);
			var rigthBoundary = Factorial(boundaries.Right);
			
			var distanceToLeftBoundary = number - leftBoundary;
			var distanceToRigthBoundary = rigthBoundary - number;
			
			if( distanceToLeftBoundary <= distanceToRigthBoundary )
			{
				return boundaries.Left;
			}
			else
			{
				return boundaries.Right;
			}
		}

		/// <summary>
		/// returns two integers for which factorial(integer1) <= number <= factorial(integer2)
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static Pair<int, int> ArcusFactorialBoundaries(this int number)
		{
			var result = ((uint)number).ArcusFactorialBoundaries();
			return new Pair<int, int>((int)result.Left, (int)result.Right);
		}
		
		/// <summary>
		/// returns two integers for which factorial(integer1) <= number <= factorial(integer2)
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static Pair<uint, uint> ArcusFactorialBoundaries(this uint number)
		{
			if( number == 0 )
			{
				return new Pair<uint, uint>(0,0);
			}
			uint biggerFactorialBase = 0;
			uint smallerFactorialBase = 0;
			//find factorial bigger than number
			for(ushort i=1;i<=MAX_FACTORIAL_BASE;++i)
			{
				if( Factorial(i) >= number )
				{
					if( ! (Factorial(i-1) <= number ) )
					{
						throw new ArithmeticException();
					}
					smallerFactorialBase = (uint)i-1;
					biggerFactorialBase =i;
					break;
				}
			}
			
			return new Pair<uint, uint>(smallerFactorialBase, biggerFactorialBase);
		}
		

		
	}
}
