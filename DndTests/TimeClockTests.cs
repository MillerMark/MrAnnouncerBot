using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DndCore;
using DndCore.Enums;
using DndCore.CoreClasses;

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
			dndTimeClock.Advance(DndTimeSpan.FromRounds(1));
			TimeSpan difference = dndTimeClock.Time - now;
			Assert.AreEqual(6, difference.TotalSeconds);


			dndTimeClock.SetTime(now);
			dndTimeClock.Advance(new DndTimeSpan(TimeMeasure.instant, 5));
			difference = dndTimeClock.Time - now;
			Assert.AreEqual(0, difference.TotalSeconds);


			dndTimeClock.SetTime(now);
			dndTimeClock.Advance(DndTimeSpan.FromMinutes(5));
			difference = dndTimeClock.Time - now;
			Assert.AreEqual(5, difference.TotalMinutes);

			dndTimeClock.SetTime(now);
			dndTimeClock.Advance(DndTimeSpan.FromHours(3));
			difference = dndTimeClock.Time - now;
			Assert.AreEqual(3, difference.TotalHours);

			dndTimeClock.SetTime(now);
			dndTimeClock.Advance(DndTimeSpan.FromDays(2));
			difference = dndTimeClock.Time - now;
			Assert.AreEqual(2, difference.TotalDays);
		}

		[TestMethod]
		public void TestDateStringConversionDayOfYearPlusLeapYears()
		{
			DndTimeClock dndTimeClock = new DndTimeClock();

			int aLeapYear = 1376;
			int aNonLeapYear = 1374;

			dndTimeClock.SetTime(aLeapYear, 214);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("Shieldmeet"));

			dndTimeClock.SetTime(aLeapYear + 1, 214);
			Assert.IsFalse(dndTimeClock.AsDndDateString().StartsWith("Shieldmeet"));
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("1st of Eleasis"));

			dndTimeClock.SetTime(aLeapYear + 2, 214);
			Assert.IsFalse(dndTimeClock.AsDndDateString().StartsWith("Shieldmeet"));
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("1st of Eleasis"));

			dndTimeClock.SetTime(aLeapYear + 3, 214);
			Assert.IsFalse(dndTimeClock.AsDndDateString().StartsWith("Shieldmeet"));
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("1st of Eleasis"));

			dndTimeClock.SetTime(aLeapYear + 4, 214);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("Shieldmeet"));
			Assert.IsFalse(dndTimeClock.AsDndDateString().StartsWith("1st of Eleasis"));

			dndTimeClock.SetTime(aNonLeapYear, 1);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("1st of Hammer"));

			dndTimeClock.SetTime(aNonLeapYear, 122);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("Greengrass"));

			dndTimeClock.SetTime(aLeapYear, 122);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("Greengrass"));

			dndTimeClock.SetTime(aNonLeapYear, 213);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("Midsummer"));

			dndTimeClock.SetTime(aLeapYear, 213);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("Midsummer"));

			dndTimeClock.SetTime(aNonLeapYear, 274);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("Highharvestide"));

			dndTimeClock.SetTime(aLeapYear, 275);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("Highharvestide"));

			dndTimeClock.SetTime(aNonLeapYear, 335);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("The Feast of the Moon"));

			dndTimeClock.SetTime(aLeapYear, 336);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("The Feast of the Moon"));

			dndTimeClock.SetTime(aNonLeapYear, 295);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("21st of Marpenoth"));

			dndTimeClock.SetTime(aLeapYear, 296);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("21st of Marpenoth"));

			dndTimeClock.SetTime(aLeapYear, 366);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("30th of Nightal"));

			dndTimeClock.SetTime(aNonLeapYear, 365);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("30th of Nightal"));

			dndTimeClock.SetTime(aLeapYear, 365);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("29th of Nightal"));

			dndTimeClock.SetTime(aNonLeapYear, 364);
			Assert.IsTrue(dndTimeClock.AsDndDateString().StartsWith("29th of Nightal"));
		}
	}
}
