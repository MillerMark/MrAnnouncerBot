using System;
using System.Linq;

namespace MapCore
{
	public interface IItemProperties
	{
		Guid Guid { get; set; }
		string FileName { get; set; }
		double X { get; set; }
		double Y { get; set; }
		double GetTop();
		double GetLeft();
		double Height { get; set; }
		double Width { get; set; }
		bool Visible { get; set; }
		bool ContainsPoint(double x, double y);
	}
}
