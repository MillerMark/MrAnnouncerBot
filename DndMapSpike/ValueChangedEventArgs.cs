using System;
using System.Linq;

namespace DndMapSpike
{
	public class TextChangedEventArgs : EventArgs
	{
		public string Value { get; set; }
		public TextChangedEventArgs()
		{

		}
	}
	public class ValueChangedEventArgs : EventArgs
	{
		public int Value { get; set; }
		public ValueChangedEventArgs()
		{

		}
	}
}

