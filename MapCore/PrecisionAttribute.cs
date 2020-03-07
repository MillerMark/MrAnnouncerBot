using System;
using System.Linq;

namespace MapCore
{
	public class PrecisionAttribute : DesignTimeAttribute
	{
		public PrecisionAttribute(int numDigits)
		{
			NumDecimalPlaces = numDigits;
		}

		public int NumDecimalPlaces { get; set; } = 8;
	}
}
