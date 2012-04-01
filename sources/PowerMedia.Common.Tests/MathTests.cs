using System;
using PowerMedia.Common.System;
using NUnit.Framework;
using PowerMedia.Common.Collections;

namespace PowerMedia.Common.Tests.System
{
	[TestFixture]
	public class MathTests
	{

        UInt64[] factorialValues = new UInt64[] { 1, 1, 2, 6, 24, 120, 720, 5040, 40320L, 362880L, 3628800L, 39916800, 479001600L, 6227020800L };
		
		[Test]
		public void ArcusFactorial_FullRangeTest()
		{
			for(uint number = 1; number<= 479001600; number+=1291)
			{
			
				uint arcusFactorial = 		number.ArcusFactorial();
                UInt64 factorial = factorialValues[arcusFactorial];
                UInt64 factorialOfNext = factorialValues[arcusFactorial + 1];
				
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
                Assert.AreEqual(factorial, factorialValues[number]);
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
