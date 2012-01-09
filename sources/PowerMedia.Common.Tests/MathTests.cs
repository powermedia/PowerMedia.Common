using System;
using PowerMedia.Common.System;
using NUnit.Framework;
using PowerMedia.Common.Collections;

namespace PowerMedia.Common.Tests.System
{
	[TestFixture]
	public class MathTests
	{
		
		private UInt64 NaiveFactorial(uint number)
		{
			UInt64 accumulator = 1;
			for(int i = 1 ; i <= number ; ++i)
			{
				accumulator*=(UInt64)i;
			}
			return accumulator;
		}
		
		[Test]
		public void ArcusFactorial_FullRangeTest()
		{
			for(uint number = 1; number<= 479001600; number+=129)
			{
			
				uint arcusFactorial = 		number.ArcusFactorial();
				UInt64 factorial = 			NaiveFactorial(arcusFactorial);
				UInt64 factorialOfNext = 	NaiveFactorial(arcusFactorial+1);
				
				Assert.LessOrEqual	(
						Math.Abs( (float) factorial 		- number ),
						Math.Abs( (float) factorialOfNext	- number )
							);
			}
		}
		
		[Test]
		public void Factorial_FullRangeTest()
		{	
			for(int number = 1; number <= 11; ++number)
			{
				int factorialOfNext = (number+1).Factorial();
				int factorial = number.Factorial();
				Assert.That(factorial>=1);
				Assert.That(factorialOfNext>=1);
				
				Assert.AreEqual(factorialOfNext, factorial * (number+1) );
				Assert.AreEqual(factorial, NaiveFactorial((uint)number));
			}
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Factorial_OutOfRangeArgument()
        {
            uint number;
            number = PowerMedia.Common.System.MathExtensions.MAX_FACTORIAL_BASE+1;
            number.Factorial();
        }
        
	}
}
