using RealSystem = System;
using System;
using NUnit.Framework;
using PowerMedia.Common.System.IO;

namespace PowerMedia.Common.Tests.System
{
	[TestFixture]
	public class PathTests
	{
		[Test]
		public void DirectorySeparatorCharTest()
		{
			switch(RealSystem.Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
					Assert.AreEqual('/', Path.DirectorySeparatorChar);
					break;
				case PlatformID.Unix:
					Assert.AreEqual('/', Path.DirectorySeparatorChar);
					break;
				default :
					Assert.AreEqual('\\', Path.DirectorySeparatorChar);
					break;						
			}
		}
	}
}
