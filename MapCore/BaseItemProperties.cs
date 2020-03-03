using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseItemProperties: IItemProperties
	{
		static BaseItemProperties()
		{

		}

		public abstract IItemProperties Copy(double deltaX, double deltaY);


		[EditableProperty]
		public bool Locked { get; set; }

		public virtual void Move(double deltaX, double deltaY)
		{
			if (Locked)
				return;

			X += deltaX;
			Y += deltaY;
		}

		public void ResetZOrder()
		{
			ZOrder = -1;
		}

		public bool HasNoZOrder()
		{
			return ZOrder == -1;
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

		public virtual double GetRight()
		{
			return X + Width / 2.0;
		}

		public virtual double GetBottom()
		{
			return Y + Height / 2.0;
		}

		public virtual bool ContainsPoint(double x, double y)
		{
			return false;
		}

		public Guid Guid { get; set; }

		public int ZOrder { get; set; } = -1;

	}
}
