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



		public BaseStampProperties()
		{

		}

		public virtual void TransferProperties(IStampProperties stamp)
		{
			base.GetPropertiesFrom(stamp);
			Visible = stamp.Visible;
			X = stamp.X;
			Y = stamp.Y;
			Height = stamp.Height;
			Width = stamp.Width;
			Locked = stamp.Locked;

			TypeName = stamp.TypeName;
			ZOrder = stamp.ZOrder;
			FlipHorizontally = stamp.FlipHorizontally;
			FlipVertically = stamp.FlipVertically;
			Rotation = stamp.Rotation;
			Scale = stamp.Scale;
			HueShift = stamp.HueShift;
			Saturation = stamp.Saturation;
			Lightness = stamp.Lightness;
			Contrast = stamp.Contrast;
			Name = stamp.Name;
			Collectible = stamp.Collectible;
			Hideable = stamp.Hideable;
			MinStrengthToMove = stamp.MinStrengthToMove;
			Cover = stamp.Cover;
			Altitude = stamp.Altitude;
			Weight = stamp.Weight;
			Value = stamp.Value;
		}
	}
}
