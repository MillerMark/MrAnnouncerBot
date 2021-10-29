using System;
using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisualProcessingTests
{

	[TestClass]
	public class NewTests
	{
		private const double RotationTolerance = 2; // 2 degrees
		private const double OpacityTolerance = 0.01; // 1%
		private const double ScaleTolerance = 0.03; // 3%

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
		public void TestScreenTrackerOrigin()
		{
			ObsTransform results = TestImageHelper.ProcessImage("ScreenTrackerOrigin");
			Assert.AreEqual(0, results.Rotation, RotationTolerance);
			Assert.AreEqual(1, results.Opacity, OpacityTolerance);
			Assert.AreEqual(1, results.Scale, 0.45 * ScaleTolerance);
			Assert.AreEqual(960, results.Origin.X, 2);
			Assert.AreEqual(540, results.Origin.Y, 2);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}

		[TestMethod]
		public void Test500pxMargins()
		{
			ObsTransform results = TestImageHelper.ProcessImage("CodingGorilla");
			Assert.AreEqual(0, results.Rotation, RotationTolerance);
			Assert.AreEqual(1, results.Opacity, OpacityTolerance);
			Assert.AreEqual(1, results.Scale, 0.45 * ScaleTolerance);
			Assert.AreEqual(960, results.Origin.X, 1102);
			Assert.AreEqual(540, results.Origin.Y, 1058);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}
	}
}
