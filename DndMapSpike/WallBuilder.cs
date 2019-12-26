using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapCore;

namespace DndMapSpike
{
	public class WallBuilder
	{
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

		void DrawHorizontalWall(WallData horizontalWallData, Layer layer)
		{
			int yAdjust = GetYAdjust(horizontalWall);
			double pixelLengthToDraw = horizontalWallData.WallLength - Walls.EndCapSize;
			double x = Tile.Width / 2 + horizontalWallData.X + Walls.EndCapIndent;
			double imageWidth = horizontalWall.Source.Width;
			while (pixelLengthToDraw > 0)
			{
				layer.DrawImageAt(horizontalWall, (int)x, horizontalWallData.Y + yAdjust, (int)Math.Min(pixelLengthToDraw, imageWidth));
				pixelLengthToDraw -= imageWidth;
				x += imageWidth;
			}
		}

		void DrawVerticalWall(WallData wallData, Layer layer)
		{
			int xAdjust = GetXAdjust(verticalWall);
			double pixelLengthToDraw = wallData.WallLength - Walls.EndCapSize;
			double y = Tile.Height / 2 + wallData.Y + Walls.EndCapIndent;
			double imageHeight = verticalWall.Source.Height;
			while (pixelLengthToDraw > 0)
			{
				layer.DrawImageAt(verticalWall, wallData.X + xAdjust, (int)y, -1, (int)Math.Min(pixelLengthToDraw, imageHeight));
				pixelLengthToDraw -= imageHeight;
				y += imageHeight;
			}
		}

		void DrawEndCaps(Map map, WallData wallData, Layer endCapLayer)
		{
			DrawCap(map, wallData.StartColumn, wallData.StartRow, endCapLayer);
			DrawCap(map, wallData.EndColumn, wallData.EndRow, endCapLayer);
		}

		private void DrawCap(Map map, int column, int row, Layer endCapLayer)
		{
			EndCapKind endCapKind = map.GetEndCapKind(column, row);
			if (endCapKind == EndCapKind.Left)
				DrawCap(column, row, endCapLayer, endCap_Left);
			else if (endCapKind == EndCapKind.Right)
				DrawCap(column, row, endCapLayer, endCap_Right);
			else if (endCapKind == EndCapKind.Bottom)
				DrawCap(column, row, endCapLayer, endCap_Bottom);
			else if (endCapKind == EndCapKind.Top)
				DrawCap(column, row, endCapLayer, endCap_Top);
			else if (endCapKind == EndCapKind.LeftTee)
				DrawCap(column, row, endCapLayer, tee_Left);
			else if (endCapKind == EndCapKind.RightTee)
				DrawCap(column, row, endCapLayer, tee_Right);
			else if (endCapKind == EndCapKind.BottomTee)
				DrawCap(column, row, endCapLayer, tee_Bottom);
			else if (endCapKind == EndCapKind.TopTee)
				DrawCap(column, row, endCapLayer, tee_Top);
			else if (endCapKind == EndCapKind.BottomLeftCorner)
				DrawCap(column, row, endCapLayer, corner_BottomLeft);
			else if (endCapKind == EndCapKind.BottomRightCorner)
				DrawCap(column, row, endCapLayer, corner_BottomRight);
			else if (endCapKind == EndCapKind.TopRightCorner)
				DrawCap(column, row, endCapLayer, corner_TopRight);
			else if (endCapKind == EndCapKind.TopLeftCorner)
				DrawCap(column, row, endCapLayer, corner_TopLeft);
			else if (endCapKind == EndCapKind.FourWayIntersection)
				DrawCap(column, row, endCapLayer, fourWayIntersection);
		}

		private void DrawCap(int startColumn, int startRow, Layer layer, Image image)
		{
			int xAdjust = GetXAdjust(image);
			int x = startColumn * Tile.Width + 1;
			double y = (startRow + 1) * Tile.Height + 2;
			layer.DrawImageAt(image, x + xAdjust, (int)y);
		}

		private int GetXAdjust(Image image)
		{
			return 3 * Tile.Width / 2 - (int)image.Source.Width / 2;
		}

		private int GetYAdjust(Image image)
		{
			return 3 * Tile.Height / 2 - (int)image.Source.Height / 2;
		}

		public void BuildWalls(Map map, Layer horizontalWallLayer, Layer verticalWallLayer, Layer endCapLayer)
		{
			for (int column = -1; column < map.NumColumns; column++)
				for (int row = -1; row < map.NumRows; row++)
				{
					if (map.HasHorizontalWallStart(column, row))
					{
						WallData wallData = map.CollectHorizontalWall(column, row);
						DrawHorizontalWall(wallData, horizontalWallLayer);
						DrawEndCaps(map, wallData, endCapLayer);
					}

					if (map.HasVerticalWallStart(column, row))
					{
						WallData wallData = map.CollectVerticalWall(column, row);
						DrawVerticalWall(wallData, verticalWallLayer);
						DrawEndCaps(map, wallData, endCapLayer);
					}
				}
		}
	}
}

