﻿using System;
using System.IO;
using System.Reflection;
using Imaging;


namespace VisualProcessingTests
{
	public class TestImageHelper
	{
		public TestImageHelper()
		{
		}

		public static VisualProcessingResults ProcessImage(string fileName)
		{
			string location = Assembly.GetExecutingAssembly().Location;
			int indexOfBinDebug = location.IndexOf("bin\\Debug");
			if (indexOfBinDebug > 0)
			{
				location = Path.Combine(location.Substring(0, indexOfBinDebug), "TestPngs");
			}
			string fullFileName = Path.Combine(location, fileName + ".png");
			return ImageUtils.GetVisualProcessingResults(fullFileName);
		}
	}
}
