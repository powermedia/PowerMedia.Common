using PowerMedia.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PowerMedia.Common.Tests.Collections.Utils
{
    [TestFixture]
    [Timeout(30)]
    public class OperationsOnCollectionTests
    {
        [Test]
        public void CartesianProductTest()
        {
            
        	var list1 = new List<string>(){"a","b","c"};
        	var list2 = new List<int>(){1,2,3,4};
	
        	var result = list1.CartesianProduct(list2);

        	int resultCount = 0;
        	foreach( var element in result )
        	{
        		++resultCount;
        	}
        	
        	Assert.AreEqual(list1.Count*list2.Count, resultCount);
        }
        
           
    }
}
