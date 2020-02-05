using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseStampProperties
	{
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
	}
}
