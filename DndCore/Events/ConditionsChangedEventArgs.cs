using System;
using System.Linq;

namespace DndCore
{
	public class ConditionsChangedEventArgs : EventArgs
	{
		public Conditions OldConditions { get; set; }
		public Conditions NewConditions { get; set; }

		public ConditionsChangedEventArgs(Conditions oldConditions, Conditions newConditions)
		{
			OldConditions = oldConditions;
			NewConditions = newConditions;
		}

		public ConditionsChangedEventArgs()
		{

		}
	}
}
