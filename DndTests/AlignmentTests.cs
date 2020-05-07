using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class AlignmentTests
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
		public void TestAllAlignments()
		{
			Assert.AreEqual(Alignment.Any, DndUtils.ToAlignment("any"));
			Assert.AreEqual(Alignment.ChaoticEvil, DndUtils.ToAlignment("chaoticEvil"));
			Assert.AreEqual(Alignment.ChaoticNeutral, DndUtils.ToAlignment("chaoticNeutral"));
			Assert.AreEqual(Alignment.ChaoticGood, DndUtils.ToAlignment("chaoticGood"));
			Assert.AreEqual(Alignment.NeutralEvil, DndUtils.ToAlignment("neutral evil"));
			Assert.AreEqual(Alignment.TrueNeutral, DndUtils.ToAlignment("neutral"));
			Assert.AreEqual(Alignment.NeutralGood, DndUtils.ToAlignment("neutral good"));
			Assert.AreEqual(Alignment.LawfulEvil, DndUtils.ToAlignment("lawful evil"));
			Assert.AreEqual(Alignment.LawfulNeutral, DndUtils.ToAlignment("lawful neutral"));
			Assert.AreEqual(Alignment.LawfulGood, DndUtils.ToAlignment("lawful good"));
			Assert.AreEqual(Alignment.Unaligned, DndUtils.ToAlignment("unaligned"));
		}
	}
}
