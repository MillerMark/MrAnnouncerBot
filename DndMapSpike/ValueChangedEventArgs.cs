using System;
using System.Linq;

namespace DndMapSpike
{
	public class ValueChangedEventArgs : EventArgs
	{
		public int Value { get; set; }
		public ValueChangedEventArgs()
		{

		}
	}
}

