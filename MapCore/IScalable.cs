using System;
using System.Linq;

namespace MapCore
{
	[Feature]
	public interface IScalable
	{
		double Scale { get; set; }

		double ScaleX { get; }
		double ScaleY { get; }
		void AdjustScale(double scaleAdjust);
		void SetAbsoluteScaleTo(double newScale);

	}
}
