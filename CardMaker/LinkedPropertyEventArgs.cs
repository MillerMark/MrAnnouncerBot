using System;
using System.Linq;

namespace CardMaker
{
	public class LinkedPropertyEventArgs : EventArgs
	{
		public double DoubleValue { get; set; }
		public bool BoolValue { get; set; }
		public string Name { get; set; }
		public LayerDetails Details { get; set; }

		public LinkedPropertyEventArgs(double value, string name, LayerDetails details)
		{
			Details = details;
			Name = name;
			DoubleValue = value;
		}

		public LinkedPropertyEventArgs(bool value, string name, LayerDetails details)
		{
			Details = details;
			Name = name;
			BoolValue = value;
		}


		public LinkedPropertyEventArgs()
		{

		}
	}
}
