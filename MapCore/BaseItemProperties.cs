using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseItemProperties
	{

		static BaseItemProperties()
		{

		}

		public bool Visible { get; set; } = true;
		public double X { get; set; }

		public double Y { get; set; }

		public abstract double Height { get; set; }
		public abstract double Width { get; set; }
		public virtual string FileName { get; set; }
	}
}
