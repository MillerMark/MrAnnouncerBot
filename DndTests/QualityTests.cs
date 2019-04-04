using DndCore;
using DndQuality;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DndTests
{
	[TestClass]
	public class QualityTests
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
		public void TestItemDescription()
		{
			Item item = new Item();
			QualityResults results = QualityEngine.CheckItem(item);
			Assert.IsNotNull(results);
			Assert.IsTrue(results.Count > 0);
			QualityIssue descriptionEmpty = results.FindFirstIssue(QualityEngine.DescriptionIsEmpty);
			Assert.IsNotNull(descriptionEmpty);
		}
	}
}
