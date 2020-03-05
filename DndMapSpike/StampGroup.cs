using System;
using MapCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DndMapSpike
{
	public class StampGroup : BaseStamp, IGroup, IFloatingItem, IStampProperties
	{
		List<IItemProperties> children = new List<IItemProperties>();
		public StampGroup(): base()
		{
			TypeName = nameof(StampGroup);
		}

		public override void ResetImage()
		{
			foreach (IItemProperties item in children)
				if (item is IStampProperties stamp)
					stamp.ResetImage();
		}

		public StampGroup(StampGroup stampGroup): this()
		{
			this.CloneGroupFrom(stampGroup);
		}

		public List<IStampProperties> OnlyStamps
		{
			get
			{
				return children.OfType<IStampProperties>().ToList();
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
				return children.Any(x => x is IStampProperties);
			}
		}

		private IStampProperties FirstStamp
		{
			get
			{
				return (children.FirstOrDefault(x => x is IStampProperties) as IStampProperties);
			}
		}

		public override string FileName
		{
			get
			{
				if (!children.Any())
					return string.Empty;
				return children.FirstOrDefault().FileName;
			}
			set
			{
				foreach (IItemProperties stamp in children)
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
				return this.GetImage();
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
			GroupHelper.BlendStampImage(this, stampsLayer, xOffset, yOffset);
		}

		public override bool ContainsPoint(double x, double y)
		{
			return GroupHelper.ContainsPoint(this, x, y);
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
			return this.Copy<StampGroup>(deltaX, deltaY);
		}

		public void Vanquish()
		{
			
		}
		public void CreateFloating(Canvas canvas, double left = 0, double top = 0)
		{
			this.CreateFloatingImages(canvas, left, top);
		}

		public void Ungroup(List<IItemProperties> ungroupedStamps)
		{
			GroupHelper.Ungroup(this, ungroupedStamps);
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
			this.CalculateSizeAndPosition(X, Y);
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
			this.CalculateSizeAndPosition(X, Y);
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
			return GroupHelper.DeserializeGroup<StampGroup>(stamp);
		}

		void IGroup.Deserialize(SerializedStamp serializedStamp)
		{
			base.Deserialize(serializedStamp);
		}

		public override double Width { get; set; }

		public override double Height { get; set; }
		public List<IItemProperties> Children { get => children; set => children = value; }
	}
}

