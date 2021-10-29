using System;
using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisualProcessingTests
{
	[TestClass]
	public class RotationTests
	{
		private const double RotationTolerance = 0.02; // 1/50th degree


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
		[DataRow("RightDegrees0.5", 0.5)]
		[DataRow("RightDegrees1", 1)]
		[DataRow("RightDegrees1.5", 1.5)]
		[DataRow("LeftDegrees0.5", -0.5)]
		[DataRow("LeftDegrees1", -1)]
		[DataRow("LeftDegrees1.5", -1.5)]
		public void TestRotation(string imageName, double expectedRotation)
		{
			ObsTransform results = TestImageHelper.ProcessImage(imageName);
			Assert.AreEqual(expectedRotation, results.Rotation, RotationTolerance);
		}
	}
}
