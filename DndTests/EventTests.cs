using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class EventTests
	{
		static EventTests()
		{
			Folders.UseTestData = true;
		}

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
		public void TestConditionsChanged()
		{
			Conditions oldConditions = Conditions.None;
			Conditions newConditions = Conditions.None;

			void Ava_ConditionsChanged(object sender, ConditionsChangedEventArgs ea)
			{
				oldConditions = ea.OldConditions;
				newConditions = ea.NewConditions;
			}

			Character ava = AllPlayers.GetFromId(PlayerID.Ava);
			ava.ConditionsChanged += Ava_ConditionsChanged;
			ava.ActiveConditions = Conditions.None;
			Assert.AreEqual(Conditions.None, oldConditions);
			Assert.AreEqual(Conditions.None, newConditions);

			ava.ActiveConditions = Conditions.Paralyzed | Conditions.Petrified;

			Assert.AreEqual(Conditions.None, oldConditions);
			Assert.AreEqual(Conditions.Paralyzed | Conditions.Petrified, newConditions);

			ava.ActiveConditions = Conditions.Prone;

			Assert.AreEqual(Conditions.Paralyzed | Conditions.Petrified, oldConditions);
			Assert.AreEqual(Conditions.Prone, newConditions);
		}
	}
}
