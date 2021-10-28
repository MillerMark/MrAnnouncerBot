using Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VisualProcessingTests
{

	[TestClass]
	/// Tests 
	public class OldTests
	{
		private const double RotationTolerance = 2; // 2 degrees
		private const double OpacityTolerance = 0.01; // 1%
		private const double ScaleTolerance = 0.03; // 3%
																									// ![](85A57DE6CCF3D489D8B5327808D66AAC.png;;31,0,226,166)

		[TestMethod]
		public void TestOrigin()
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage("Origin");
			Assert.AreEqual(0, results.Rotation, RotationTolerance);
			Assert.AreEqual(1201, results.Origin.X, 10);
			Assert.AreEqual(1054, results.Origin.Y, 10);
			Assert.AreEqual(1, results.Opacity, OpacityTolerance);
			Assert.AreEqual(0.25, results.Scale, ScaleTolerance);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}

		// ![](Origin42Opacity;;1121,921,1296,1079)

		[TestMethod]
		public void TestOrigin42PercentOpacity()
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage("Origin - 42% opacity");
			Assert.AreEqual(0, results.Rotation, RotationTolerance);
			Assert.AreEqual(1201, results.Origin.X, 10);
			Assert.AreEqual(1054, results.Origin.Y, 10);
			Assert.AreEqual(0.25, results.Scale, ScaleTolerance);
			Assert.AreEqual(0.42, results.Opacity, OpacityTolerance);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}

		// ![](45)
		[TestMethod]
		public void Test45()
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage("45");
			Assert.AreEqual(45, results.Rotation, RotationTolerance);
			Assert.AreEqual(0.45, results.Opacity, OpacityTolerance);
			Assert.AreEqual(0.45/4, results.Scale, 0.45 * ScaleTolerance);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}


		// ![](45 Left 55 Scale 65 Opacity)
		[TestMethod]
		public void Test45Left()
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage("45 Left 55 Scale 65 Opacity");
			Assert.AreEqual(76, results.Origin.X, 5);
			Assert.AreEqual(65, results.Origin.Y, 5);
			Assert.AreEqual(-45, results.Rotation, RotationTolerance);
			Assert.AreEqual(0.65, results.Opacity, OpacityTolerance);
			Assert.AreEqual(0.55/4, results.Scale, 0.55 * ScaleTolerance);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}


		// ![](180 Rotation 200 Scale 79 Opacity)
		[TestMethod]
		public void Test180()
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage("180 Rotation 200 Scale 79 Opacity");
			Assert.AreEqual(180, results.Rotation, RotationTolerance);
			Assert.AreEqual(220, results.Origin.X, 5);
			Assert.AreEqual(48, results.Origin.Y, 5);
			Assert.AreEqual(0.79, results.Opacity, OpacityTolerance);
			Assert.AreEqual(0.5, results.Scale, 2 * ScaleTolerance);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}

		// ![](37Flipped)
		[TestMethod]
		public void Test37Flipped()
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage("37Flipped");
			Assert.AreEqual(37, results.Rotation, 3);
			Assert.AreEqual(40, results.Origin.X, 3);
			Assert.AreEqual(70, results.Origin.Y, 3);
			Assert.AreEqual(0.37, results.Opacity, OpacityTolerance);
			Assert.AreEqual(0.37/4, results.Scale, 0.37 * ScaleTolerance);
			Assert.AreEqual(true, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}

		// ![](53Flipped)
		[TestMethod]
		public void Test53Flipped()
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage("53Flipped");
			Assert.AreEqual(-53, results.Rotation, 3);
			Assert.AreEqual(61, results.Origin.X, 3);
			Assert.AreEqual(54, results.Origin.Y, 3);
			Assert.AreEqual(0.53, results.Opacity, OpacityTolerance);
			Assert.AreEqual(0.53/4, results.Scale, 0.53 * ScaleTolerance);
			Assert.AreEqual(true, results.Flipped);
			Assert.AreEqual(0, results.Camera);
		}

		// ![](SideCamera90)
		[TestMethod]
		public void TestSideCamera90()
		{
			LiveFeedSequence results = TestImageHelper.ProcessImage("SideCamera90");
			Assert.AreEqual(90, results.Rotation, 3);
			Assert.AreEqual(23, results.Origin.X, 3);
			Assert.AreEqual(85, results.Origin.Y, 3);
			Assert.AreEqual(0.9, results.Opacity, OpacityTolerance);
			Assert.AreEqual(0.9/4, results.Scale, 0.9 * ScaleTolerance);
			Assert.AreEqual(false, results.Flipped);
			Assert.AreEqual(1, results.Camera);
		}
		
	}
}
