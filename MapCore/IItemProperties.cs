using System;
using System.Linq;

namespace MapCore
{
	public interface IItemProperties: IGuid
	{
		IItemProperties Copy(double deltaX, double deltaY);
		string TypeName { get; set; }
		string FileName { get; set; }
		int ZOrder { get; set; }
		double X { get; set; }
		double Y { get; set; }
		void Move(double deltaX, double deltaY);
		double GetTop();
		double GetLeft();
		double GetBottom();
		double GetRight();

		double Height { get; set; }
		double Width { get; set; }
		bool Visible { get; set; }
		bool Locked { get; set; }

		bool ContainsPoint(double x, double y);
		bool HasNoZOrder();
		void ResetZOrder();
	}
}
