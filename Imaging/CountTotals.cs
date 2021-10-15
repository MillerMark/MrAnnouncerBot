using System;
using System.Drawing;
using System.Collections.Generic;

namespace Imaging
{
	public class CountTotals
	{
		byte greatestOpacityFoundSoFar;
		public double CenterY { get; set; }
		public double CenterX { get; set; }

		public int Top { get; set; } = int.MaxValue;
		public int Left { get; set; } = int.MaxValue;
		public int Bottom { get; set; } = int.MinValue;
		public int Right { get; set; } = int.MinValue;

		public int Count { get; set; }
		public double XTotal { get; set; }
		public double YTotal { get; set; }


		public double weightedSumX { get; set; }
		public double weightedSumY { get; set; }
		public double totalMass { get; set; }

		public double GetRoughX()
		{
			return XTotal / Count;
		}
		
		public double GetRoughY()
		{
			return YTotal / Count;
		}

		public double WeightedCenterX()
		{
			return weightedSumX / totalMass;
		}

		public double WeightedCenterY()
		{
			return weightedSumY / totalMass;
		}

		public void Add(int x, int y, byte opacity)
		{
			if (opacity > greatestOpacityFoundSoFar)
				greatestOpacityFoundSoFar = opacity;
			if (y > Bottom)
				Bottom = y;
			if (x > Right)
				Right = x;
			if (y < Top)
				Top = y;
			if (x < Left)
				Left = x;

			// Old algorithm
			XTotal += x;
			YTotal += y;
			Count++;

			// Pudding's algorithm:
			weightedSumX += x * opacity;
			weightedSumY += y * opacity;
			totalMass += opacity;
		}

		public void FindCircleCenters(DirectBitmap bitmap)
		{
			GetCenterColumn(bitmap);
			GetCenterRow(bitmap);
		}

		private void GetCenterColumn(DirectBitmap bitmap)
		{
			byte highestOpacity = 0;
			int indexOfHighestOpacityLine = -1;
			if (Left == int.MaxValue)
			{
				CenterY = int.MinValue;
				return;
			}
			for (int i = Left; i < Right; i++)
			{
				Color topPixel = bitmap.GetPixel(i, Top);
				if (topPixel.A > highestOpacity)
				{
					highestOpacity = topPixel.A;
					indexOfHighestOpacityLine = i;
				}
			}

			if (Top == Bottom)
			{
				CenterY = Top;
			}
			else if (highestOpacity > 0)
			{
				Color firstPixel = bitmap.GetPixel(indexOfHighestOpacityLine, Top);
				Color lastPixel = bitmap.GetPixel(indexOfHighestOpacityLine, Bottom);
				double start = Top + 1 - (double)firstPixel.A / greatestOpacityFoundSoFar;
				double bottom = Bottom - 1 + (double)lastPixel.A / greatestOpacityFoundSoFar;
				CenterY = (start + bottom) / 2;
			}
		}

		private void GetCenterRow(DirectBitmap bitmap)
		{
			byte highestOpacity = 0;
			int indexOfHighestOpacityLine = -1;
			if (Top == int.MaxValue)
			{
				CenterX = int.MinValue;
				return;
			}
			for (int i = Top; i < Bottom; i++)
			{
				Color leftPixel = bitmap.GetPixel(Left, i);
				if (leftPixel.A > highestOpacity)
				{
					highestOpacity = leftPixel.A;
					indexOfHighestOpacityLine = i;
				}
			}

			if (Left == Right)
			{
				CenterX = Left;
			}
			else if (highestOpacity > 0)
			{
				Color firstPixel = bitmap.GetPixel(Left, indexOfHighestOpacityLine);
				Color lastPixel = bitmap.GetPixel(Right, indexOfHighestOpacityLine);
				double left = Left + 1 - (double)firstPixel.A / greatestOpacityFoundSoFar;
				double right = Right - 1 + (double)lastPixel.A / greatestOpacityFoundSoFar;
				CenterX = (left + right) / 2;
			}
		}
	}
}
