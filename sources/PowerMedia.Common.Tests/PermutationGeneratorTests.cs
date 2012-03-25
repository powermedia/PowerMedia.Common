using PowerMedia.Common.Data;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace PowerMedia.Common.Tests.Data
{
	[TestFixture]
	public class PermutationGeneratorTests
	{
		[Test]
        [Category("Heavy")]
        [Timeout(5000)]
        public void TestPermutation()
        {
            List<int> elements = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            
            var set = PermutationGenerator.GetPermutations(elements);
            // 9! = 362 880
            Assert.AreEqual(362880, set.Count);

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPermutation_NullArgument()
        {
            PermutationGenerator.GetPermutations<int>(null);
        }
	}
}
