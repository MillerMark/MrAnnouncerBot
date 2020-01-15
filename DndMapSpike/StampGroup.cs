using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class StampGroup : IStamp
	{
		List<IStamp> stamps = new List<IStamp>();
		public StampGroup()
		{

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
					stamp.FlipHorizontally = value;
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
					stamp.FlipVertically = value;
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

		//public int RelativeX
		//{
		//	get
		//	{
		//		return stamps.FirstOrDefault().RelativeX;
		//	}
		//	set
		//	{
		//		foreach (IStamp stamp in stamps)
		//		{
		//			stamp.RelativeX = value;
		//		}
		//	}
		//}
		//public int RelativeY
		//{
		//	get
		//	{
		//		return stamps.FirstOrDefault().RelativeY;
		//	}
		//	set
		//	{
		//		foreach (IStamp stamp in stamps)
		//		{
		//			stamp.RelativeY = value;
		//		}
		//	}
		//}

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
		public double Scale
		{
			get
			{
				return stamps.FirstOrDefault().Scale;
			}
			set
			{
				foreach (IStamp stamp in stamps)
				{
					stamp.Scale = value;
				}
			}
		}

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

		public int ZOrder { get; set; }

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

		public bool HasNoZOrder()
		{
			return ZOrder == -1;
		}

		public void Move(int deltaX, int deltaY)
		{
			X += deltaX;
			Y += deltaY;
		}

		public void ResetZOrder()
		{
			ZOrder = -1;
		}

		public void RotateLeft()
		{
			foreach (IStamp stamp in stamps)
				stamp.RotateLeft();
		}

		public void RotateRight()
		{
			foreach (IStamp stamp in stamps)
				stamp.RotateRight();
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
			throw new NotImplementedException();
		}

		void CalculateSizeAndPosition()
		{
			int leftMost = stamps.Min(x => x.GetLeft());
			int topMost = stamps.Min(x => x.GetTop());
			int rightMost = stamps.Max(x => x.GetLeft() + x.Width);
			int bottomMost = stamps.Max(x => x.GetTop() + x.Height);
			ZOrder = stamps.Max(x => x.ZOrder);

			Width = rightMost - leftMost;
			Height = bottomMost - topMost;
			int middleX = (leftMost + rightMost) / 2;
			int middleY = (topMost + bottomMost) / 2;
			X = middleX;
			Y = middleY;
			foreach (IStamp stamp in stamps)
			{
				stamp.X -= middleX;
				stamp.Y -= middleY;
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
			return result;
		}

		public void Vanquish()
		{
			
		}
		public void CreateFloating(Canvas canvas, int x = 0, int y = 0)
		{
			foreach (IStamp stamp in stamps)
				stamp.CreateFloating(canvas, stamp.X + x, stamp.Y + y);
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

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int X { get; set; }
		public int Y { get; set; }

	}
}

