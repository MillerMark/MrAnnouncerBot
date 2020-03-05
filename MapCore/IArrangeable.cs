using System;
using System.Linq;

namespace MapCore
{
	[Feature]
	public interface IArrangeable
	{
		bool FlipHorizontally { get; set; }
		bool FlipVertically { get; set; }
		StampRotation Rotation { get; set; }
		void RotateLeft();
		void RotateRight();
		void SwapXY();
	}
}
