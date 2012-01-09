using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PowerMedia.Common.System.Reflection;

namespace PowerMedia.Common.Tests
{
    [TestFixture]
    public class ReflectionUtilsTests
    {
        private interface ITestInterface
        {
        }
        private class TestClass : ITestInterface
        {
            public TestClass TestProperty { get; set; }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoesImplementInterface_NullArgument_Type()
        {
            Type type = null;
            ReflectionUtils.DoesImplementInterface(type, null);            
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoesImplementInterface_NullArgument_Object()
        {
            Object obj = null;
            ReflectionUtils.DoesImplementInterface(obj, null);
        }
      
    }
}
