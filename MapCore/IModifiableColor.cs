using System;
using System.Linq;

namespace MapCore
{
	public interface IModifiableColor
	{
		double Contrast { get; set; }
		double HueShift { get; set; }
		double Lightness { get; set; }
		double Saturation { get; set; }
	}
}
