using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class ExpressionTests
	{
		static ExpressionTests()
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
		public void Test()
		{
			Assert.AreEqual(3, Expressions.Get("1 + 2", null));
			Assert.IsTrue((bool)Expressions.Get("true | false", null));
			Assert.IsTrue((bool)Expressions.Get("false | (false | (false | true)) & (false | true)", null));
			Assert.IsFalse((bool)Expressions.Get("false | false", null));
		}
	}
}
