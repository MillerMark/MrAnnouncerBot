using System;
using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisualProcessingTests
{
	[TestClass]
	public class TestCameras
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

		// ![](Camera1Right90;;;0.01091,0.01091)

		[TestMethod]
		public void TestCamera1Right90()
		{
			ObsTransform results = TestImageHelper.ProcessImage("Camera1Right90");
			Assert.AreEqual(90, results.Rotation);
			Assert.AreEqual(1, results.Opacity);
			Assert.AreEqual(1, results.Scale);
			Assert.AreEqual(784, results.Origin.X, 0.15);
			Assert.AreEqual(364, results.Origin.Y, 0.15);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(1, results.Camera);
		}

		//` ![](Camera2-180;;;0.01289,0.01289)
		[TestMethod]
		public void TestCamera2_180()
		{
			ObsTransform results = TestImageHelper.ProcessImage("Camera2-180");
			Assert.AreEqual(180, results.Rotation);
			Assert.AreEqual(1, results.Opacity);
			Assert.AreEqual(1, results.Scale);
			Assert.AreEqual(960, results.Origin.X, 0.15);
			Assert.AreEqual(188, results.Origin.Y, 0.15);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(2, results.Camera);
		}


		//`![](Camera3-270;;;0.01261,0.01261)
		[TestMethod]
		public void TestCamera3_270()
		{
			ObsTransform results = TestImageHelper.ProcessImage("Camera3-270");
			Assert.AreEqual(-90, results.Rotation);
			Assert.AreEqual(1, results.Opacity);
			Assert.AreEqual(1, results.Scale);
			Assert.AreEqual(1136, results.Origin.X, 0.15);
			Assert.AreEqual(364, results.Origin.Y, 0.15);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(3, results.Camera);
		}
	}
}
