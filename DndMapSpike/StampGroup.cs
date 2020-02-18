using System;
using MapCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DndMapSpike
{
	public class StampGroup : BaseStamp, IStamp, IStampGroup
	{
		List<IStampProperties> stamps = new List<IStampProperties>();
		public StampGroup()
		{
			TypeName = nameof(StampGroup);
		}

		public override void ResetImage()
		{
			foreach (IStampProperties stamp in stamps)
				stamp.ResetImage();
		}

		public StampGroup(StampGroup stampGroup): base()
		{
			X = stampGroup.X;
			Y = stampGroup.Y;
			Width = stampGroup.Width;
			Height = stampGroup.Height;
			foreach (IStampProperties stamp in stampGroup.stamps)
				stamps.Add(stamp.Copy(0, 0));
		}

		public override double Contrast
		{
			get
			{
				if (!stamps.Any())
					return 0;
				return stamps.FirstOrDefault().Contrast;
			}
			set
			{
				foreach (IStampProperties stamp in stamps)
				{
					stamp.Contrast = value;
				}
			}
		}
		public override string FileName
		{
			get
			{
				return stamps.FirstOrDefault().FileName;
			}
			set
			{
				foreach (IStampProperties stamp in stamps)
				{
					stamp.FileName = value;
				}
			}
		}

		public override bool FlipHorizontally
		{
			get
			{
				return stamps.FirstOrDefault().FlipHorizontally;
			}
			set
			{
				foreach (IStampProperties stamp in stamps)
				{
					stamp.FlipHorizontally = !stamp.FlipHorizontally;
					stamp.X *= -1;
				}
			}
		}

		public override bool FlipVertically
		{
			get
			{
				return stamps.FirstOrDefault().FlipVertically;
			}
			set
			{
				foreach (IStampProperties stamp in stamps)
				{
					stamp.FlipVertically = !stamp.FlipVertically;
					stamp.Y *= -1;
				}
			}
		}

		public override double HueShift
		{
			get
			{
				if (stamps.Count == 0)
					return 0;
				return stamps.FirstOrDefault().HueShift;
			}
			set
			{
				foreach (IStampProperties stamp in stamps)
				{
					stamp.HueShift = value;
				}
			}
		}

		[JsonIgnore]
		public Image Image
		{
			get
			{
				if (stamps.FirstOrDefault() is IStamp stamp)
					return stamp.Image;
				return null;
			}
		}

		public override double Lightness
		{
			get
			{
				if (!stamps.Any())
					return 0;
				return stamps.FirstOrDefault().Lightness;
			}
			set
			{
				foreach (IStampProperties stamp in stamps)
				{
					stamp.Lightness = value;
				}
			}
		}

		public override StampRotation Rotation
		{
			get
			{
				return stamps.FirstOrDefault().Rotation;
			}
			set
			{
				foreach (IStampProperties stamp in stamps)
				{
					stamp.Rotation = value;
				}
			}
		}

		public override double Saturation
		{
			get
			{
				if (!stamps.Any())
					return 0;
				return stamps.FirstOrDefault().Saturation;
			}
			set
			{
				foreach (IStampProperties stamp in stamps)
				{
					stamp.Saturation = value;
				}
			}
		}

		public override double Scale { get; set; } = 1;

		public override double ScaleX
		{
			get
			{
				return stamps.FirstOrDefault().ScaleX;
			}
		}
		public override double ScaleY
		{
			get
			{
				return stamps.FirstOrDefault().ScaleY;
			}
		}

		public void BlendStampImage(StampsLayer stampsLayer, int xOffset = 0, int yOffset = 0)
		{
			if (!Visible)
				return;
			foreach (IStampProperties stamp in stamps)
				if (stamp is IStamp wpfStamp)
					wpfStamp.BlendStampImage(stampsLayer, xOffset + X, yOffset + Y);
		}

		public override bool ContainsPoint(double x, double y)
		{
			Point relativeTestPoint = new Point(x - X, y - Y);
			return stamps.Any(s => s.ContainsPoint(relativeTestPoint.X, relativeTestPoint.Y));
		}

		public override void Move(int deltaX, int deltaY)
		{
			X += deltaX;
			Y += deltaY;
		}

		public override void RotateLeft()
		{
			foreach (IStampProperties stamp in stamps)
			{
				stamp.SwapXY();
				stamp.Y *= -1;
				stamp.RotateLeft();
			}
			SwapHeightAndWidth();
		}

		public override void RotateRight()
		{
			foreach (IStampProperties stamp in stamps)
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

		public override int GetLeft()
		{
			return (int)Math.Round(X - Width / 2.0);
		}

		/// <summary>
		/// Gets the top of this group (X and Y are center points)
		/// </summary>
		/// <returns></returns>
		public override int GetTop()
		{
			return (int)Math.Round(Y - Height / 2.0);
		}

		public override IStampProperties Copy(int deltaX, int deltaY)
		{
			StampGroup result = new StampGroup(this);
			result.Move(deltaX, deltaY);
			return result;
		}

		void CalculateSizeAndPosition(int xOffset = 0, int yOffset = 0)
		{
			if (stamps.Count == 0)
				return;
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
			foreach (IStampProperties stamp in stamps)
			{
				stamp.X -= X;
				stamp.Y -= Y;
			}
		}

		public static StampGroup Create(List<IStampProperties> stamps)
		{
			StampGroup result = new StampGroup();
			List<IStampProperties> sortedStamps = stamps.OrderBy(x => x.ZOrder).ToList();
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
			foreach (IStampProperties stamp in stamps)
				if (stamp is IStamp wpfStamp)
					wpfStamp.CreateFloating(canvas, stamp.GetLeft() + left + Width / 2, stamp.GetTop() + top + Height / 2);
		}

		public void Ungroup(List<IStampProperties> ungroupedStamps)
		{
			for (int i = 0; i < stamps.Count; i++)
			{
				IStampProperties stamp = stamps[i];
				stamp.Move(X, Y);
				stamp.ZOrder = ZOrder + i;
				ungroupedStamps.Add(stamp);
			}
		}
		public override void AdjustScale(double scaleAdjust)
		{
			Scale *= scaleAdjust;
			foreach (IStampProperties stamp in stamps)
			{
				stamp.X = (int)Math.Round(stamp.X * scaleAdjust);
				stamp.Y = (int)Math.Round(stamp.Y * scaleAdjust);
				stamp.AdjustScale(scaleAdjust);
			}
			CalculateSizeAndPosition(X, Y);
		}

		public override void SetAbsoluteScaleTo(double newScale)
		{
			double scaleAdjust = newScale / Scale;
			Scale = newScale;
			foreach (IStampProperties stamp in stamps)
			{
				stamp.X = (int)Math.Round(stamp.X * scaleAdjust);
				stamp.Y = (int)Math.Round(stamp.Y * scaleAdjust);
				stamp.AdjustScale(scaleAdjust);
			}
			CalculateSizeAndPosition(X, Y);
		}

		public override double GetBottom()
		{
			return Y + Height / 2;
		}

		public override double GetRight()
		{
			return X + Width / 2;
		}
		public static StampGroup From(SerializedStamp stamp)
		{
			StampGroup stampGroup = new StampGroup();
			stampGroup.TransferFrom(stamp);
			if (stamp.Children != null)
				foreach (SerializedStamp childStamp in stamp.Children)
					stampGroup.Stamps.Add(CreateStampFrom(childStamp));
			return stampGroup;
		}

		public override int Width { get; set; }

		public override int Height { get; set; }
		public List<IStampProperties> Stamps { get => stamps; set => stamps = value; }
	}
}

