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
		public void TestOrigin()
		{
			VisualProcessingResults results = TestImageHelper.ProcessImage("Origin");
			Assert.AreEqual(0, results.Rotation, 5);
			Assert.AreEqual(1201, results.Origin.X, 10);
			Assert.AreEqual(1054, results.Origin.Y, 10);
			Assert.AreEqual(1, results.Opacity, 0.1);
		}

		// ![](Origin42Opacity;;1121,921,1296,1079)

		[TestMethod]
		public void TestOrigin42PercentOpacity()
		{
			VisualProcessingResults results = TestImageHelper.ProcessImage("Origin - 42% opacity");
			Assert.AreEqual(0, results.Rotation, 5);
			Assert.AreEqual(1201, results.Origin.X, 10);
			Assert.AreEqual(1054, results.Origin.Y, 10);
			Assert.AreEqual(0.42, results.Opacity, 0.1);
		}
	}
}
