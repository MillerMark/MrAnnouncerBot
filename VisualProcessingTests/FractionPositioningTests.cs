using System;
using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisualProcessingTests
{
	[TestClass]
	public class FractionPositioningTests
	{
		private const double PositionalTolerance = 0.105;  // Almost 1/10th of a pixel!!!
		private const double OpacityTolerance = 0.015;
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

		[DataTestMethod]
		[DataRow("ScreenCenter", 960, 540)]
		[DataRow("960.5, 540.5", 960.5, 540.5)]
		[DataRow("960.8, 540.8", 960.8, 540.8)]
		[DataRow("777", 777.7, 777.7, 7.7)]
		[DataRow("888.8", 888.8, 888.8, 26)]
		[DataRow("888", 888.8, 888.8, 8.8, 0.888)]
		[DataRow("666", 666.6, 666.6, 66.6, 0.666)]
		[DataRow("555", 555.5, 555.5, 55.5, 0.555)]
		[DataRow("444", 444.4, 444.4, 44.4, 0.444)]
		public void TestTrackerImage(string fileName, double expectedX, double expectedY, double expectedRotation = 0, double expectedScale = 1)
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage(fileName);
			Assert.AreEqual(expectedX, results.Origin.X, PositionalTolerance);
			Assert.AreEqual(expectedY, results.Origin.Y, PositionalTolerance);
			Assert.AreEqual(expectedRotation, results.Rotation);
			Assert.AreEqual(expectedScale, results.Scale);
		}

		[DataTestMethod]
		[DataRow("777Opacity", 777.7, 777.7, 77.7, 0.777, 0.077)]
		public void TestTrackerImageWithOpacity(string fileName, double expectedX, double expectedY, double expectedRotation = 0, double expectedScale = 1, double expectedOpacity = 1)
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage(fileName);
			Assert.AreEqual(expectedX, results.Origin.X, PositionalTolerance);
			Assert.AreEqual(expectedY, results.Origin.Y, PositionalTolerance);
			Assert.AreEqual(expectedRotation, results.Rotation);
			Assert.AreEqual(expectedScale, results.Scale);
			Assert.AreEqual(expectedOpacity, results.Opacity, OpacityTolerance);
		}
	}
}
