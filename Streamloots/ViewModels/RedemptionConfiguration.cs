using System;
using System.Linq;

namespace Streamloots
{
	public class RedemptionConfiguration
	{
		// TODO: Verify this initial value is valid and determine for what it is used (must be positive to avoid errors).
		public int timeFrameSeconds { get; set; } = 1;
		public RedemptionConfiguration()
		{

		}
	}
}
