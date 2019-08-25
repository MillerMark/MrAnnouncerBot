using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class StrUtilTests
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
		public void TestStrExtensionMethods()
		{
			string testStr = "1d4(cold)";
			Assert.AreEqual(1, testStr.GetFirstInt());
			Assert.AreEqual(4, testStr.EverythingAfter("d").GetFirstInt());
			Assert.AreEqual("1d4", testStr.EverythingBefore("("));
			Assert.AreEqual(0.166667, "0.166667d8".GetFirstDouble());
			Assert.AreEqual("(cold)", "(" + testStr.EverythingAfter("("));
			Assert.AreEqual("cold", testStr.EverythingAfter("(").EverythingBefore(")"));
		}
	}
}
