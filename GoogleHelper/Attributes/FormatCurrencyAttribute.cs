using System;
using System.Linq;

namespace GoogleHelper
{
	public class FormatCurrencyAttribute : FormatAttribute
	{
		public FormatCurrencyAttribute(string pattern) : base(pattern)
		{

		}
	}
}
