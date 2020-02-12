using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseStampProperties
	{
		public Guid Guid { get; set; }

		public string TypeName { get; set; }

		public bool Visible { get; set; } = true;
		public int X { get; set; }

		public int Y { get; set; }

		public int ZOrder { get; set; } = -1;

		public abstract int Height { get; set; }
		public abstract int Width { get; set; }

		public virtual bool FlipHorizontally { get; set; }
		public virtual bool FlipVertically { get; set; }
		public virtual StampRotation Rotation { get; set; }
		public virtual string FileName { get; set; }
		public virtual double Scale { get; set; } = 1;
		public virtual double HueShift { get; set; }
		public virtual double Saturation { get; set; }
		public virtual double Lightness { get; set; }
		public virtual double Contrast { get; set; }

		public BaseStampProperties()
		{

		}

		public virtual void TransferProperties(IStampProperties stampProperties)
		{
			TypeName = stampProperties.TypeName;
			Guid = stampProperties.Guid;
			Visible = stampProperties.Visible;
			X = stampProperties.X;
			Y = stampProperties.Y;
			ZOrder = stampProperties.ZOrder;
			Height = stampProperties.Height;
			Width = stampProperties.Width;
			FlipHorizontally = stampProperties.FlipHorizontally;
			FlipVertically = stampProperties.FlipVertically;
			Rotation = stampProperties.Rotation;
			FileName = stampProperties.FileName;
			Scale = stampProperties.Scale;
			HueShift = stampProperties.HueShift;
			Saturation = stampProperties.Saturation;
			Lightness = stampProperties.Lightness;
			Contrast = stampProperties.Contrast;
		}
		protected virtual void TransferFrom(SerializedStamp serializedStamp)
		{
			TypeName = serializedStamp.TypeName;
			Guid = serializedStamp.Guid;
			Visible = serializedStamp.Visible;
			X = serializedStamp.X;
			Y = serializedStamp.Y;
			ZOrder = serializedStamp.ZOrder;
			Height = serializedStamp.Height;
			Width = serializedStamp.Width;
			FlipHorizontally = serializedStamp.FlipHorizontally;
			FlipVertically = serializedStamp.FlipVertically;
			Rotation = serializedStamp.Rotation;
			FileName = serializedStamp.FileName;
			Scale = serializedStamp.Scale;
			HueShift = serializedStamp.HueShift;
			Saturation = serializedStamp.Saturation;
			Lightness = serializedStamp.Lightness;
			Contrast = serializedStamp.Contrast;
		}
	}
}
