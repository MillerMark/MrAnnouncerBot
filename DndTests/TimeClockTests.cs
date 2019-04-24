using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DndCore;

namespace DndTests
{
	[TestClass]
	public class TimeClockTests
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		[TestMethod]
		public void TestInit()
		{
			DndTimeClock dndTimeClock = new DndTimeClock();
			DateTime now = DateTime.Now;
			dndTimeClock.SetTime(now);
			dndTimeClock.Advance(new DndTimeSpan(TimeMeasure.turns, 1));
			TimeSpan difference = dndTimeClock.Time - now;
			Assert.AreEqual(6, difference.TotalSeconds);


			dndTimeClock.SetTime(now);
			dndTimeClock.Advance(new DndTimeSpan(TimeMeasure.instant, 5));
			difference = dndTimeClock.Time - now;
			Assert.AreEqual(0, difference.TotalSeconds);


			dndTimeClock.SetTime(now);
			dndTimeClock.Advance(new DndTimeSpan(TimeMeasure.minutes, 5));
			difference = dndTimeClock.Time - now;
			Assert.AreEqual(5, difference.TotalMinutes);
		}
	}
}
