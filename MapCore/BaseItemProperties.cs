using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseItemProperties: IItemProperties
	{

		static BaseItemProperties()
		{

		}

		public virtual void TransferProperties(IItemProperties stampProperties)
		{
			Guid = stampProperties.Guid;
			X = stampProperties.X;
			Y = stampProperties.Y;
			Visible = stampProperties.Visible;
			FileName = stampProperties.FileName;
			Height = stampProperties.Height;
			Width = stampProperties.Width;
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

	}
}
