using System;
using System.Linq;

namespace GoogleHelper
{
	public class FormatDateAttribute : FormatAttribute
	{
		public FormatDateAttribute(string pattern) : base(pattern)
		{

		}
	}
}
