using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class StampGroup : BaseStamp, IStamp
	{
		List<IStamp> stamps = new List<IStamp>();
		public StampGroup()
		{

		}

		public StampGroup(StampGroup stampGroup)
		{
			X = stampGroup.X;
			Y = stampGroup.Y;
			Width = stampGroup.Width;
			Height = stampGroup.Height;
			foreach (IStamp stamp in stampGroup.stamps)
				stamps.Add(stamp.Copy(0, 0));
		}

		public double Contrast
		{
			get
			{
				return stamps.FirstOrDefault().Contrast;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.Contrast = value;
				}
			}
		}
		public string FileName
		{
			get
			{
				return stamps.FirstOrDefault().FileName;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.FileName = value;
				}
			}
		}

		public bool FlipHorizontally
		{
			get
			{
				return stamps.FirstOrDefault().FlipHorizontally;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.FlipHorizontally = !stamp.FlipHorizontally;
					stamp.X *= -1;
				}
			}
		}

		public bool FlipVertically
		{
			get
			{
				return stamps.FirstOrDefault().FlipVertically;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.FlipVertically = !stamp.FlipVertically;
					stamp.Y *= -1;
				}
			}
		}

		public double HueShift
		{
			get
			{
				return stamps.FirstOrDefault().HueShift;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.HueShift = value;
				}
			}
		}

		public Image Image  // Double check code accessing this property..
		{
			get
			{
				return stamps.FirstOrDefault().Image;
			}
		}

		public double Lightness
		{
			get
			{
				return stamps.FirstOrDefault().Lightness;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.Lightness = value;
				}
			}
		}

		public StampRotation Rotation
		{
			get
			{
				return stamps.FirstOrDefault().Rotation;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.Rotation = value;
				}
			}
		}

		public double Saturation
		{
			get
			{
				return stamps.FirstOrDefault().Saturation;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.Saturation = value;
				}
			}
		}

		public double Scale { get; set; } = 1;

		public double ScaleX
		{
			get
			{
				return stamps.FirstOrDefault().ScaleX;
			}
		}
		public double ScaleY
		{
			get
			{
				return stamps.FirstOrDefault().ScaleY;
			}
		}

		public void BlendStampImage(StampsLayer stampsLayer, int xOffset = 0, int yOffset = 0)
		{
			foreach (IStamp stamp in stamps)
				stamp.BlendStampImage(stampsLayer, xOffset + X, yOffset + Y);
		}

		public bool ContainsPoint(Point point)
		{
			Point relativeTestPoint = new Point(point.X - X, point.Y - Y);
			return stamps.Any(x => x.ContainsPoint(relativeTestPoint));
		}

		public void Move(int deltaX, int deltaY)
		{
			X += deltaX;
			Y += deltaY;
		}

		public void RotateLeft()
		{
			foreach (IStamp stamp in stamps)
			{
				stamp.SwapXY();
				stamp.Y *= -1;
				stamp.RotateLeft();
			}
			SwapHeightAndWidth();
		}

		public void RotateRight()
		{
			foreach (IStamp stamp in stamps)
			{
				stamp.SwapXY();
				stamp.X *= -1;
				stamp.RotateRight();
			}
			SwapHeightAndWidth();
		}

		private void SwapHeightAndWidth()
		{
			int oldWidth = Width;
			Width = Height;
			Height = oldWidth;
		}

		public int GetLeft()
		{
			return (int)Math.Round(X - Width / 2.0);
		}

		/// <summary>
		/// Gets the top of this group (X and Y are center points)
		/// </summary>
		/// <returns></returns>
		public int GetTop()
		{
			return (int)Math.Round(Y - Height / 2.0);
		}

		public IStamp Copy(int deltaX, int deltaY)
		{
			StampGroup result = new StampGroup(this);
			result.Move(deltaX, deltaY);
			return result;
		}

		void CalculateSizeAndPosition(int xOffset = 0, int yOffset = 0)
		{
			int leftMost = stamps.Min(x => x.GetLeft() + xOffset);
			int topMost = stamps.Min(x => x.GetTop() + yOffset);
			int rightMost = stamps.Max(x => x.GetLeft() + xOffset + x.Width);
			int bottomMost = stamps.Max(x => x.GetTop() + yOffset + x.Height);
			ZOrder = stamps.Max(x => x.ZOrder);

			Width = rightMost - leftMost;
			Height = bottomMost - topMost;
			int middleX = (leftMost + rightMost) / 2;
			int middleY = (topMost + bottomMost) / 2;
			X = middleX;
			Y = middleY;
		}

		private void PositionContainedStampsRelativeToCenter()
		{
			foreach (IStamp stamp in stamps)
			{
				stamp.X -= X;
				stamp.Y -= Y;
			}
		}

		public static StampGroup Create(List<IStamp> stamps)
		{
			StampGroup result = new StampGroup();
			List<IStamp> sortedStamps = stamps.OrderBy(x => x.ZOrder).ToList();
			for (int i = 0; i < sortedStamps.Count; i++)
			{
				sortedStamps[i].ZOrder = i + 1;
			}
			result.stamps = sortedStamps;

			result.CalculateSizeAndPosition();
			result.PositionContainedStampsRelativeToCenter();
			return result;
		}

		public void Vanquish()
		{
			
		}
		public void CreateFloating(Canvas canvas, int left = 0, int top = 0)
		{
			foreach (IStamp stamp in stamps)
				stamp.CreateFloating(canvas, stamp.GetLeft() + left + Width / 2, stamp.GetTop() + top + Height / 2);
		}

		public void Ungroup(List<IStamp> ungroupedStamps)
		{
			for (int i = 0; i < stamps.Count; i++)
			{
				IStamp stamp = stamps[i];
				stamp.Move(X, Y);
				stamp.ZOrder = ZOrder + i;
				ungroupedStamps.Add(stamp);
			}
		}
		public void AdjustScale(double scaleAdjust)
		{
			Scale *= scaleAdjust;
			foreach (IStamp stamp in stamps)
			{
				stamp.X = (int)Math.Round(stamp.X * scaleAdjust);
				stamp.Y = (int)Math.Round(stamp.Y * scaleAdjust);
				stamp.AdjustScale(scaleAdjust);
			}
			CalculateSizeAndPosition(X, Y);
		}

		public void SetAbsoluteScaleTo(double newScale)
		{
			double scaleAdjust = newScale / Scale;
			Scale = newScale;
			foreach (IStamp stamp in stamps)
			{
				stamp.X = (int)Math.Round(stamp.X * scaleAdjust);
				stamp.Y = (int)Math.Round(stamp.Y * scaleAdjust);
				stamp.AdjustScale(scaleAdjust);
			}
			CalculateSizeAndPosition(X, Y);
		}

		public int Width { get; private set; }

		public int Height { get; private set; }

	}
}

