using System;
using NUnit.Framework;
using PowerMedia.Common.Diagnostics;
using System.Threading;


namespace PowerMedia.Common.Tests.Diagnostics
{
	[TestFixture]
	public class TimeMeasurerTests
	{
		[Test]
		public void Constructor_Test()
		{
			TimeMeasurer _timeMeasurer = new TimeMeasurer();
			Assert.IsNotNull(_timeMeasurer);
			Assert.IsNull(_timeMeasurer.AverageTime);
		}
		
		[Test]
		public void Measurer_Test()
		{
			TimeMeasurer _timeMeasurer = new TimeMeasurer();
			_timeMeasurer.Start();
			_timeMeasurer.Stop();
			if(_timeMeasurer.AverageTime != null && _timeMeasurer.AverageTime.HasValue)
			{
				Assert.LessOrEqual(0, _timeMeasurer.AverageTime.Value);
			}
			_timeMeasurer.Reset();
			Assert.IsNull(_timeMeasurer.AverageTime);
		}
				
	}
}
