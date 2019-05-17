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
			ItemViewModel item = new ItemViewModel();
			QualityResults results = QualityEngine.CheckItem(item, "This isn't needed.");
			Assert.IsNotNull(results);
			Assert.IsTrue(results.Count > 0);
			QualityIssue descriptionEmpty = results.FindFirstIssue(QualityEngine.DescriptionIsEmpty);
			Assert.IsNotNull(descriptionEmpty);
		}
	}
}
