using PowerMedia.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PowerMedia.Common.Tests.Collections.Utils
{
    [TestFixture]
    public class PermutationGeneratorTests
    {
        [Test]
        public void TestPermutation()
        {
            List<int> elements = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var set = PermutationGenerator.GetPermutations<List<int>, int>(elements);
            Assert.AreEqual(362880, set.Count);

        }
    }
}
