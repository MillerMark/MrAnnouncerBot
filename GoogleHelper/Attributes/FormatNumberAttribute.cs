using System;
using System.Linq;

namespace GoogleHelper
{
/*
 Examples of custom number formats:

 ![](F0F266A01FBB2779146BFF2FAE889852.png;https://www.benlcollins.com/spreadsheets/google-sheets-custom-number-format/ ;826,337,1488,713)  << Click to see more.

 */
	public class FormatNumberAttribute: FormatAttribute
	{
		public FormatNumberAttribute(string pattern): base(pattern)
		{
		}
	}
}
