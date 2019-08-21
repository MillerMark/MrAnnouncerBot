using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class AlarmTests
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
		public void Test()
		{
			bool firstAlarmFired = false;
			bool secondAlarmFired = false;
			DateTime firstAlarmTime = DateTime.MinValue;
			DateTime secondAlarmTime = DateTime.MinValue;



			void First_AlarmFired(object sender, DndTimeEventArgs ea)
			{
				firstAlarmTime = ea.TimeClock.Time;
				firstAlarmFired = true;
			}
			void Second_AlarmFired(object sender, DndTimeEventArgs ea)
			{
				secondAlarmTime = ea.TimeClock.Time;
				secondAlarmFired = true;
			}

			DndTimeClock dndTimeClock = new DndTimeClock();
			DateTime startTime = new DateTime(2000, 1, 1);
			dndTimeClock.SetTime(startTime);
			DndAlarm first = dndTimeClock.CreateAlarm(TimeSpan.FromSeconds(5), "First");
			first.AlarmFired += First_AlarmFired;
			DndAlarm second = dndTimeClock.CreateAlarm(TimeSpan.FromSeconds(15), "Second");
			second.AlarmFired += Second_AlarmFired;
			
			Assert.IsFalse(firstAlarmFired);
			Assert.IsFalse(secondAlarmFired);
			Assert.AreEqual(firstAlarmTime, DateTime.MinValue);
			Assert.AreEqual(secondAlarmTime, DateTime.MinValue);
			dndTimeClock.Advance(DndTimeSpan.FromSeconds(10));
			Assert.IsTrue(firstAlarmFired);
			Assert.AreEqual(firstAlarmTime, startTime + TimeSpan.FromSeconds(5));
			Assert.IsFalse(secondAlarmFired);

			dndTimeClock.Advance(DndTimeSpan.FromSeconds(10));
			Assert.IsTrue(secondAlarmFired);
			Assert.AreEqual(secondAlarmTime, startTime + TimeSpan.FromSeconds(15));
		}


	}
}
