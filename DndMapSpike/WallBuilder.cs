using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using MapCore;

namespace DndMapSpike
{
	public class WallBuilder
	{
		Image insideCornerVoidTopLeft;
		Image insideCornerVoidTopRight;
		Image insideCornerVoidBottomLeft;
		Image insideCornerVoidBottomRight;
		Image outsideCornerVoidTopLeft;
		Image outsideCornerVoidTopRight;
		Image outsideCornerVoidBottomLeft;
		Image outsideCornerVoidBottomRight;
		Image horizontalVoidTop;
		Image horizontalVoidBottom;
		Image verticalVoidLeft;
		Image verticalVoidRight;
		Image horizontalWall;
		Image verticalWall;
		Image endCap_Left;
		Image endCap_Right;
		Image endCap_Top;
		Image endCap_Bottom;
		Image tee_Top;
		Image tee_Bottom;
		Image tee_Left;
		Image tee_Right;
		Image corner_TopLeft;
		Image corner_BottomLeft;
		Image corner_BottomRight;
		Image corner_TopRight;
		Image fourWayIntersection;
		public WallBuilder()
		{
			LoadImages();
		}

		void LoadImages()
		{
			insideCornerVoidTopLeft = LoadImage(@"InnerVoid\Inside-TopLeft.png");
			insideCornerVoidTopRight = LoadImage(@"InnerVoid\Inside-TopRight.png");
			insideCornerVoidBottomLeft = LoadImage(@"InnerVoid\Inside-BottomLeft.png");
			insideCornerVoidBottomRight = LoadImage(@"InnerVoid\Inside-BottomRight.png");
			outsideCornerVoidTopLeft = LoadImage(@"InnerVoid\Outside-TopLeft.png");
			outsideCornerVoidTopRight = LoadImage(@"InnerVoid\Outside-TopRight.png");
			outsideCornerVoidBottomLeft = LoadImage(@"InnerVoid\Outside-BottomLeft.png");
			outsideCornerVoidBottomRight = LoadImage(@"InnerVoid\Outside-BottomRight.png");
			horizontalVoidTop = LoadImage(@"InnerVoid\HorizontalTop.png");
			horizontalVoidBottom = LoadImage(@"InnerVoid\HorizontalBottom.png");
			verticalVoidLeft = LoadImage(@"InnerVoid\VerticalLeft.png");
			verticalVoidRight = LoadImage(@"InnerVoid\VerticalRight.png");
			horizontalWall = LoadImage("HorizontalWall.png");
			verticalWall = LoadImage("VerticalWall.png");
			endCap_Left = LoadImage("EndCap_Left.png");
			endCap_Right = LoadImage("EndCap_Right.png");
			endCap_Top = LoadImage("EndCap_Top.png");
			endCap_Bottom = LoadImage("EndCap_Bottom.png");
			tee_Top = LoadImage("T_Top.png");
			tee_Bottom = LoadImage("T_Bottom.png");
			tee_Left = LoadImage("T_Left.png");
			tee_Right = LoadImage("T_Right.png");
			corner_TopLeft = LoadImage("Corner_TopLeft.png");
			corner_BottomLeft = LoadImage("Corner_BottomLeft.png");
			corner_BottomRight = LoadImage("Corner_BottomRight.png");
			corner_TopRight = LoadImage("Corner_TopRight.png");
			fourWayIntersection = LoadImage("Intersection.png");
		}
		Image LoadImage(string assetName)
		{
			string imagePath = System.IO.Path.Combine(TextureUtils.WallFolder, assetName);
			Image image = new Image();
			image.Source = new BitmapImage(new Uri(imagePath));
			return image;
		}

		void DrawHorizontalWall(WallData horizontalWallData, Layer layer, Image horizontalWallImage, int additionalPixelsToDraw = 0)
		{
			double pixelLengthToDraw = horizontalWallData.WallLength - Walls.EndCapSize + 1 + additionalPixelsToDraw;
			double x = Tile.Width / 2 + horizontalWallData.X + Walls.EndCapIndent;
			DrawHorizontalWall(horizontalWallImage, x, horizontalWallData.Y, pixelLengthToDraw, layer);
		}

		void DrawHorizontalWall(Image image, double x, double y, double pixelLengthToDraw, Layer layer)
		{
			double yAdjust = GetWallYAdjust(image);
			double imageWidth = image.Source.Width;
			while (Math.Round(pixelLengthToDraw) > 0)
			{
				layer.BlendImage(image, x, y + yAdjust, Math.Min(pixelLengthToDraw, imageWidth));
				pixelLengthToDraw -= imageWidth;
				x += imageWidth;
			}
		}

		void DrawVerticalWall(WallData wallData, Layer layer, Image verticalWallImage)
		{
			double pixelLengthToDraw = wallData.WallLength - Walls.EndCapSize + 1;
			double y = Tile.Height / 2 + wallData.Y + Walls.EndCapIndent;
			DrawVerticalWall(verticalWallImage, wallData.X, y, pixelLengthToDraw, layer);
		}

		void DrawVerticalWall(Image image, double x, double y, double pixelLengthToDraw, Layer layer)
		{
			double xAdjust = GetWallXAdjust(image);
			double imageHeight = image.Source.Height;
			while (Math.Round(pixelLengthToDraw) > 0)
			{
				layer.BlendImage(image, x + xAdjust, y, -1, Math.Min(pixelLengthToDraw, imageHeight));
				pixelLengthToDraw -= imageHeight;
				y += imageHeight;
			}
		}

		void DrawEndCaps(Map map, WallData wallData, Layer endCapLayer)
		{
			DrawCap(map, wallData.StartColumn, wallData.StartRow, endCapLayer);
			DrawCap(map, wallData.EndColumn, wallData.EndRow, endCapLayer);
		}

		Image GetEndCapImage(EndCapKind endCapKind)
		{
			switch (endCapKind)
			{
				case EndCapKind.Left:
					return endCap_Left;
				case EndCapKind.Right:
					return endCap_Right;
				case EndCapKind.Bottom:
					return endCap_Bottom;
				case EndCapKind.Top:
					return endCap_Top;
				case EndCapKind.LeftTee:
					return tee_Left;
				case EndCapKind.RightTee:
					return tee_Right;
				case EndCapKind.BottomTee:
					return tee_Bottom;
				case EndCapKind.TopTee:
					return tee_Top;
				case EndCapKind.BottomLeftCorner:
					return corner_BottomLeft;
				case EndCapKind.BottomRightCorner:
					return corner_BottomRight;
				case EndCapKind.TopRightCorner:
					return corner_TopRight;
				case EndCapKind.TopLeftCorner:
					return corner_TopLeft;
				case EndCapKind.FourWayIntersection:
					return fourWayIntersection;
			}
			return null;
		}

		private void DrawCap(Map map, int column, int row, Layer endCapLayer)
		{
			EndCapKind endCapKind = map.GetEndCapKind(column, row);

			if (endCapKind == EndCapKind.None)
				return;

			if (alreadyDrewEndCap[column + 1, row + 1])
				return;

			Image capImage = GetEndCapImage(endCapKind);
			DrawCap(column, row, endCapLayer, capImage);
			alreadyDrewEndCap[column + 1, row + 1] = true;
		}

		private void DrawCap(int startColumn, int startRow, Layer layer, Image image)
		{
			if (image == null)
				return;
			double x = GetWallImageStartX(startColumn, image);
			double y = GetWallImageStartY(startRow, image);
			layer.BlendImage(image, x, y);
		}

		double GetWallImageStartY(int startRow, Image image)
		{
			double yAdjust = GetWallYAdjust(image);
			double y = startRow * Tile.Height;
			return y + yAdjust;
		}

		private double GetWallImageStartX(int startColumn, Image image)
		{
			double xAdjust = GetWallXAdjust(image);
			double x = startColumn * Tile.Width + 2;
			return x + xAdjust;
		}

		private double GetWallXAdjust(Image image)
		{
			return (3 * Tile.Width - image.Source.Width) / 2;
		}

		private double GetWallYAdjust(Image image)
		{
			return (3 * Tile.Height - image.Source.Height) / 2;
		}

		void DrawVoidCap(int column, int row, VoidCornerKind voidKind, Layer voidLayer)
		{
			if (alreadyDrewVoidCap[column + 1, row + 1])
				return;

			Image voidCapImage = null;

			switch (voidKind)
			{
				case VoidCornerKind.TLeft:
					voidCapImage = verticalVoidRight;
					break;
				case VoidCornerKind.TTop:
					voidCapImage = horizontalVoidBottom;
					break;
				case VoidCornerKind.TRight:
					voidCapImage = verticalVoidLeft;
					break;
				case VoidCornerKind.TBottom:
					voidCapImage = horizontalVoidTop;
					//DrawHorizontalWall()
					break;
			}
			if (voidCapImage != null)   // It's a T
			{
				double x = GetWallImageStartX(column, voidCapImage);
				double y = GetWallImageStartY(row, voidCapImage);
				voidLayer.BlendImage(voidCapImage, x, y);
			}
			else
			{
				voidCapImage = GetVoidCapImage(voidKind);
				if (voidCapImage == null)
					return;

				DrawCap(column, row, voidLayer, voidCapImage);
			}
			alreadyDrewVoidCap[column + 1, row + 1] = true;
		}

		Image GetVoidCapImage(VoidCornerKind startCap)
		{
			switch (startCap)
			{
				case VoidCornerKind.None:
					return null;
				case VoidCornerKind.InsideTopLeft:
					return insideCornerVoidTopLeft;
				case VoidCornerKind.InsideTopRight:
					return insideCornerVoidTopRight;
				case VoidCornerKind.InsideBottomLeft:
					return insideCornerVoidBottomLeft;
				case VoidCornerKind.InsideBottomRight:
					return insideCornerVoidBottomRight;
				case VoidCornerKind.OutsideTopLeft:
					return outsideCornerVoidTopLeft;
				case VoidCornerKind.OutsideTopRight:
					return outsideCornerVoidTopRight;
				case VoidCornerKind.OutsideBottomLeft:
					return outsideCornerVoidBottomLeft;
				case VoidCornerKind.OutsideBottomRight:
					return outsideCornerVoidBottomRight;
			}
			return null;
		}
		bool IsTVoid(VoidCornerKind voidKind)
		{
			switch (voidKind)
			{
				case VoidCornerKind.TLeft:
				case VoidCornerKind.TTop:
				case VoidCornerKind.TRight:
				case VoidCornerKind.TBottom:
					return true;
			}
			return false;
		}
		void DrawInnerVoid(Map map, WallData wallData, Layer voidLayer)
		{
			VoidCornerKind startVoidKind = map.GetVoidKind(wallData.StartColumn, wallData.StartRow);
			VoidCornerKind endVoidKind = map.GetVoidKind(wallData.EndColumn, wallData.EndRow);
			if (wallData.Orientation == WallOrientation.Vertical)
			{
				Image voidImage = null;
				if (map.VoidIsLeft(wallData.StartColumn, wallData.StartRow))
					voidImage = verticalVoidRight;
				else if (map.VoidIsRight(wallData.StartColumn, wallData.StartRow))
					voidImage = verticalVoidLeft;
				if (voidImage != null)
					DrawVerticalWall(wallData, voidLayer, voidImage);
				if (IsTVoid(startVoidKind))
				{
					DrawVoidCap(wallData.StartColumn, wallData.StartRow, startVoidKind, voidLayer);
				}
				if (IsTVoid(endVoidKind))
				{
					DrawVoidCap(wallData.EndColumn, wallData.EndRow, endVoidKind, voidLayer);
				}
			}
			else
			{
				Image voidImage = null;
				if (map.VoidIsAbove(wallData.StartColumn, wallData.StartRow))
					voidImage = horizontalVoidBottom;
				else if (map.VoidIsBelow(wallData.StartColumn, wallData.StartRow))
					voidImage = horizontalVoidTop;
				if (voidImage != null)
					DrawHorizontalWall(wallData, voidLayer, voidImage, 1);

				if (startVoidKind != VoidCornerKind.None)
					DrawVoidCap(wallData.StartColumn, wallData.StartRow, startVoidKind, voidLayer);

				if (endVoidKind != VoidCornerKind.None)
					DrawVoidCap(wallData.EndColumn, wallData.EndRow, endVoidKind, voidLayer);
			}
		}
		bool[,] alreadyDrewEndCap;
		bool[,] alreadyDrewVoidCap;
		public void BuildWalls(Map map, Layer wallLayer)
		{
			alreadyDrewEndCap = new bool[map.NumColumns + 2, map.NumRows + 2];
			alreadyDrewVoidCap = new bool[map.NumColumns + 2, map.NumRows + 2];
			List<WallData> innerVoids = new List<WallData>();
			List<WallData> endCaps = new List<WallData>();
			for (int column = -1; column < map.NumColumns; column++)
				for (int row = -1; row < map.NumRows; row++)
				{
					if (map.HasHorizontalWallStart(column, row))
					{
						WallData wallData = map.CollectHorizontalWall(column, row);
						DrawHorizontalWall(wallData, wallLayer, horizontalWall);
						endCaps.Add(wallData);
						innerVoids.Add(wallData);
					}

					if (map.HasVerticalWallStart(column, row))
					{
						WallData wallData = map.CollectVerticalWall(column, row);
						DrawVerticalWall(wallData, wallLayer, verticalWall);
						endCaps.Add(wallData);
						innerVoids.Add(wallData);
					}
				}

			foreach (WallData wallData in innerVoids)
				DrawEndCaps(map, wallData, wallLayer);

			foreach (WallData wallData in innerVoids)
				DrawInnerVoid(map, wallData, wallLayer);
		}
	}
}

