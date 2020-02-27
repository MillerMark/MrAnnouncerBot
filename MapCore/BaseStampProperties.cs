using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseStampProperties
	{
		//! TODO: Make sure new properties are dealt with in the Clone method!!!

		[EditableProperty]
		public string Name { get; set; }

		[EditableProperty]
		public bool Collectible { get; set; }
		[EditableProperty]
		public bool Locked { get; set; }

		[EditableProperty]
		public bool Hideable { get; set; }

		[EditableProperty]
		public bool Moveable { get; set; }

		[EditableProperty("Min Strength to Move:", "Moveable", 0)]
		public double MinStrengthToMove { get; set; } = 0;

		[EditableProperty]
		public Cover Cover { get; set; }

		[EditableProperty]
		public StampAltitude Altitude { get; set; } = StampAltitude.Mid;

		[EditableProperty]
		public double Weight { get; set; }

		/// <summary>
		/// In Gold Pieces
		/// </summary>
		[EditableProperty(2)]
		public double Value { get; set; } = 1; // GP

		public Guid Guid { get; set; }

		public string TypeName { get; set; }

		public bool Visible { get; set; } = true;
		public double X { get; set; }

		public double Y { get; set; }

		public int ZOrder { get; set; } = -1;

		public abstract double Height { get; set; }
		public abstract double Width { get; set; }

		public virtual bool FlipHorizontally { get; set; }
		public virtual bool FlipVertically { get; set; }
		public virtual StampRotation Rotation { get; set; }
		public virtual string FileName { get; set; }
		public virtual double Scale { get; set; } = 1;
		public virtual double HueShift { get; set; }
		public virtual double Saturation { get; set; }
		public virtual double Lightness { get; set; }
		public virtual double Contrast { get; set; }

		public void CloneFrom(BaseStampProperties stamp)
		{
			Contrast = stamp.Contrast;
			Visible = stamp.Visible;
			FlipHorizontally = stamp.FlipHorizontally;
			FlipVertically = stamp.FlipVertically;
			HueShift = stamp.HueShift;
			Lightness = stamp.Lightness;
			Rotation = stamp.Rotation;
			Saturation = stamp.Saturation;
			Scale = stamp.Scale;
			X = stamp.X;
			Y = stamp.Y;
			Height = stamp.Height;
			Width = stamp.Width;
			Name = stamp.Name;
			Collectible = stamp.Collectible;
			Locked = stamp.Locked;
			Hideable = stamp.Hideable;
			MinStrengthToMove = stamp.MinStrengthToMove;
			Cover = stamp.Cover;
			Altitude = stamp.Altitude;
			Weight = stamp.Weight;
			Value = stamp.Value;
			TypeName = stamp.TypeName;
		}

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
			Name = stampProperties.Name;
			Collectible = stampProperties.Collectible;
			Hideable = stampProperties.Hideable;
			MinStrengthToMove = stampProperties.MinStrengthToMove;
			Cover = stampProperties.Cover;
			Altitude = stampProperties.Altitude;
			Weight = stampProperties.Weight;
			Value = stampProperties.Value;
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
			Name = serializedStamp.Name;
			Collectible = serializedStamp.Collectible;
			Hideable = serializedStamp.Hideable;
			MinStrengthToMove = serializedStamp.MinStrengthToMove;
			Cover = serializedStamp.Cover;
			Altitude = serializedStamp.Altitude;
			Weight = serializedStamp.Weight;
			Value = serializedStamp.Value;
		}
	}
}
