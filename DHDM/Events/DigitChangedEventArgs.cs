using System;
using System.Linq;

namespace DHDM
{
	public class DigitChangedEventArgs : EventArgs
	{
		public DigitChangedEventArgs(string keyword, decimal value)
		{
			Value = value;
			Keyword = keyword;
		}
		public string Keyword { get; set; }
		public decimal Value { get; set; }
	}
}
