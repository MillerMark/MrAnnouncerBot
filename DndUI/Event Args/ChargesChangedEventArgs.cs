using System;
using System.Linq;

namespace DndUI
{
	public class ChargesChangedEventArgs : EventArgs
	{
		public ChargesChangedEventArgs(string key, int newValue)
		{
			Key = key;
			NewValue = newValue;
		}

		public int NewValue { get; set; }
		public string Key { get; set; }
	}
}
