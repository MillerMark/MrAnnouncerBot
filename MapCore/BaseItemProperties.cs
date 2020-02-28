using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseItemProperties: IItemProperties
	{
		static BaseItemProperties()
		{

		}

		public virtual void TransferProperties(IItemProperties itemProperties)
		{
			Guid = itemProperties.Guid;
			X = itemProperties.X;
			Y = itemProperties.Y;
			ZOrder = itemProperties.ZOrder;
			Visible = itemProperties.Visible;
			FileName = itemProperties.FileName;
			Height = itemProperties.Height;
			Width = itemProperties.Width;
		}

		public bool Visible { get; set; } = true;
		public double X { get; set; }

		public double Y { get; set; }

		public abstract double Height { get; set; }
		public abstract double Width { get; set; }
		public virtual string FileName { get; set; }

		public virtual double GetLeft()
		{
			return X - Width / 2.0;
		}

		public virtual double GetTop()
		{
			return Y - Height / 2.0;
		}

		public virtual bool ContainsPoint(double x, double y)
		{
			return false;
		}


		public Guid Guid { get; set; }

		public int ZOrder { get; set; } = -1;

	}
}
