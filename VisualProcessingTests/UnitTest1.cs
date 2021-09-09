using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VisualProcessingTests
{
	[TestClass]
	public class UnitTest1
	{
		// ![](85A57DE6CCF3D489D8B5327808D66AAC.png;;31,0,226,166)

		[TestMethod]
		public void TestMethod1()
		{
			VisualProcessingResults results = TestImageHelper.ProcessImage("Origin");
			Assert.AreEqual(0, results.Rotation, 5);
			Assert.AreEqual(1201, results.X, 10);
			Assert.AreEqual(1054, results.Y, 10);
			Assert.AreEqual(1, results.Opacity, 0.1);
		}
	}
}
