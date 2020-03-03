using System;
using MapCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DndMapSpike
{
	public class StampGroup : BaseStamp, IFloatingItem, IStampProperties, IItemGroup
	{
		List<IItemProperties> items = new List<IItemProperties>();
		public StampGroup()
		{
			TypeName = nameof(StampGroup);
		}

		public override void ResetImage()
		{
			foreach (IItemProperties item in items)
				if (item is IStampProperties stamp)
					stamp.ResetImage();
		}

		public StampGroup(StampGroup stampGroup): base()
		{
			X = stampGroup.X;
			Y = stampGroup.Y;
			Width = stampGroup.Width;
			Height = stampGroup.Height;
			foreach (IItemProperties item in items)
				items.Add(item.Clone());
		}

		public List<IStampProperties> OnlyStamps
		{
			get
			{
				return items.OfType<IStampProperties>().ToList();
			}
		}

		public override double Contrast
		{
			get
			{
				if (!HasAnyStamps)
					return 0;
				return FirstStamp.Contrast;
			}
			set
			{
				if (Locked)
					return;

				foreach (IStampProperties stamp in OnlyStamps)
				{
					stamp.Contrast = value;
				}
			}
		}

		private bool HasAnyStamps
		{
			get
			{
				return items.Any(x => x is IStampProperties);
			}
		}

		private IStampProperties FirstStamp
		{
			get
			{
				return (items.FirstOrDefault(x => x is IStampProperties) as IStampProperties);
			}
		}

		public override string FileName
		{
			get
			{
				if (!items.Any())
					return string.Empty;
				return items.FirstOrDefault().FileName;
			}
			set
			{
				foreach (IItemProperties stamp in items)
				{
					stamp.FileName = value;
				}
			}
		}

		public override bool FlipHorizontally
		{
			get
			{
				if (!HasAnyStamps)
					return false;
				return FirstStamp.FlipHorizontally;
			}
			set
			{
				if (Locked)
					return;

				foreach (IStampProperties stamp in OnlyStamps)
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
				if (!HasAnyStamps)
					return false;
				return FirstStamp.FlipVertically;
			}
			set
			{
				if (Locked)
					return;

				foreach (IStampProperties stamp in OnlyStamps)
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
				if (!HasAnyStamps)
					return 0;
				return FirstStamp.HueShift;
			}
			set
			{
				if (Locked)
					return;

				foreach (IStampProperties stamp in OnlyStamps)
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
				if (items.FirstOrDefault() is IFloatingItem stamp)
					return stamp.Image;
				return null;
			}
		}

		public override double Lightness
		{
			get
			{
				if (!HasAnyStamps)
					return 0;
				return FirstStamp.Lightness;
			}
			set
			{
				if (Locked)
					return;

				foreach (IStampProperties stamp in OnlyStamps)
				{
					stamp.Lightness = value;
				}
			}
		}

		public override StampRotation Rotation
		{
			get
			{
				if (!HasAnyStamps)
					return 0;
				return FirstStamp.Rotation;
			}
			set
			{
				if (Locked)
					return;

				foreach (IStampProperties stamp in OnlyStamps)
				{
					stamp.Rotation = value;
				}
			}
		}

		public override double Saturation
		{
			get
			{
				if (!HasAnyStamps)
					return 0;
				return FirstStamp.Saturation;
			}
			set
			{
				if (Locked)
					return;

				foreach (IStampProperties stamp in OnlyStamps)
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
				if (!HasAnyStamps)
					return 1;
				return FirstStamp.ScaleX;
			}
		}
		public override double ScaleY
		{
			get
			{
				if (!HasAnyStamps)
					return 1;
				return FirstStamp.ScaleY;
			}
		}

		public void BlendStampImage(StampsLayer stampsLayer, double xOffset = 0, double yOffset = 0)
		{
			if (!Visible)
				return;
			foreach (IItemProperties stamp in items)
				if (stamp is IFloatingItem floatingItem)
					floatingItem.BlendStampImage(stampsLayer, xOffset + X, yOffset + Y);
		}

		public override bool ContainsPoint(double x, double y)
		{
			Point relativeTestPoint = new Point(x - X, y - Y);
			return items.Any(s => s.ContainsPoint(relativeTestPoint.X, relativeTestPoint.Y));
		}

		public override void Move(double deltaX, double deltaY)
		{
			if (Locked)
				return;
			X += deltaX;
			Y += deltaY;
		}

		public override void RotateLeft()
		{
			if (Locked)
				return;

			foreach (IStampProperties stamp in OnlyStamps)
			{
				stamp.SwapXY();
				stamp.Y *= -1;
				stamp.RotateLeft();
			}
			SwapHeightAndWidth();
		}

		public override void RotateRight()
		{
			if (Locked)
				return;
			foreach (IStampProperties stamp in OnlyStamps)
			{
				stamp.SwapXY();
				stamp.X *= -1;
				stamp.RotateRight();
			}
			SwapHeightAndWidth();
		}

		private void SwapHeightAndWidth()
		{
			double oldWidth = Width;
			Width = Height;
			Height = oldWidth;
		}

		public override double GetLeft()
		{
			return X - Width / 2.0;
		}

		/// <summary>
		/// Gets the top of this group (X and Y are center points)
		/// </summary>
		/// <returns></returns>
		public override double GetTop()
		{
			return Y - Height / 2.0;
		}

		public override IItemProperties Copy(double deltaX, double deltaY)
		{
			StampGroup result = new StampGroup(this);
			result.Move(deltaX, deltaY);
			return result;
		}

		void CalculateSizeAndPosition(double xOffset = 0, double yOffset = 0)
		{
			if (items.Count == 0)
				return;
			double leftMost = items.Min(x => x.GetLeft() + xOffset);
			double topMost = items.Min(x => x.GetTop() + yOffset);
			double rightMost = items.Max(x => x.GetLeft() + xOffset + x.Width);
			double bottomMost = items.Max(x => x.GetTop() + yOffset + x.Height);
			ZOrder = items.Max(x => x.ZOrder);

			Width = rightMost - leftMost;
			Height = bottomMost - topMost;
			X = (leftMost + rightMost) / 2.0;
			Y = (topMost + bottomMost) / 2.0;
		}

		private void PositionContainedStampsRelativeToCenter()
		{
			foreach (IItemProperties stamp in items)
			{
				stamp.X -= X;
				stamp.Y -= Y;
			}
		}

		public static StampGroup Create(List<IItemProperties> stamps)
		{
			StampGroup result = new StampGroup();
			List<IItemProperties> sortedStamps = stamps.OrderBy(x => x.ZOrder).ToList();
			for (int i = 0; i < sortedStamps.Count; i++)
			{
				sortedStamps[i].ZOrder = i + 1;
			}
			result.items = sortedStamps;

			result.CalculateSizeAndPosition();
			result.PositionContainedStampsRelativeToCenter();
			return result;
		}

		public void Vanquish()
		{
			
		}
		public void CreateFloating(Canvas canvas, double left = 0, double top = 0)
		{
			foreach (IItemProperties stamp in items)
				if (stamp is IFloatingItem floatingItem)
					floatingItem.CreateFloating(canvas, stamp.GetLeft() + left + Width / 2.0, stamp.GetTop() + top + Height / 2.0);
		}

		public void Ungroup(List<IItemProperties> ungroupedStamps)
		{
			if (Locked)
				return;

			for (int i = 0; i < items.Count; i++)
			{
				IItemProperties stamp = items[i];
				stamp.Move(X, Y);
				stamp.ZOrder = ZOrder + i;
				ungroupedStamps.Add(stamp);
			}
		}
		public override void AdjustScale(double scaleAdjust)
		{
			if (Locked)
				return;

			Scale *= scaleAdjust;
			foreach (IStampProperties stamp in OnlyStamps)
			{
				stamp.X = stamp.X * scaleAdjust;
				stamp.Y = stamp.Y * scaleAdjust;
				stamp.AdjustScale(scaleAdjust);
			}
			CalculateSizeAndPosition(X, Y);
		}

		public override void SetAbsoluteScaleTo(double newScale)
		{
			if (Locked)
				return;

			double scaleAdjust = newScale / Scale;
			Scale = newScale;
			foreach (IStampProperties stamp in OnlyStamps)
			{
				stamp.X = stamp.X * scaleAdjust;
				stamp.Y = stamp.Y * scaleAdjust;
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
					stampGroup.Stamps.Add(MapElementFactory.CreateStampFrom(childStamp));
			return stampGroup;
		}

		public override double Width { get; set; }

		public override double Height { get; set; }
		public List<IItemProperties> Stamps { get => items; set => items = value; }
	}
}

