using System;
using System.Linq;

namespace Streamloots
{
	public class RedemptionLimit
	{
		public RedemptionConfiguration configuration { get; set; }
		public DateTime resetAt { get; set; }
		public RedemptionLimit()
		{

		}
	}
}
