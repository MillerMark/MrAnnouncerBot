using System;
using System.Linq;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class FeatureTests
	{
		static FeatureTests()
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
		public void TestLoad()
		{
			Feature barbarianMelee = AllFeatures.Get("BarbarianMelee");
			Assert.IsNotNull(barbarianMelee);
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Assert.IsFalse(Expressions.GetBool("BarbarianMelee(strength)", fred));
			Expressions.Do("Set(Rage,true)", fred);
			// TODO: Left off here.
		}
	}
}
