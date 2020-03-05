using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseStampProperties: BaseItemProperties
	{
		//! TODO: Make sure new properties are dealt with in the Clone method!!!

		[EditableProperty]
		public string Name { get; set; }

		[EditableProperty]
		public bool Collectible { get; set; }
		
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

		public virtual bool FlipHorizontally { get; set; }
		public virtual bool FlipVertically { get; set; }
		public virtual StampRotation Rotation { get; set; }
		public virtual double Scale { get; set; } = 1;
		public virtual double HueShift { get; set; }
		public virtual double Saturation { get; set; }
		public virtual double Lightness { get; set; }
		public virtual double Contrast { get; set; }

		public virtual void CloneFrom(IStampProperties stamp)
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
			base.TransferProperties(stampProperties);
			TypeName = stampProperties.TypeName;
			ZOrder = stampProperties.ZOrder;
			FlipHorizontally = stampProperties.FlipHorizontally;
			FlipVertically = stampProperties.FlipVertically;
			Rotation = stampProperties.Rotation;
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
		protected override void Deserialize(SerializedStamp serializedStamp)
		{
			base.Deserialize(serializedStamp);
			FlipHorizontally = serializedStamp.FlipHorizontally;
			FlipVertically = serializedStamp.FlipVertically;
			Rotation = serializedStamp.Rotation;
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
