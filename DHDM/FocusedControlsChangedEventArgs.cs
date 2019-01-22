using System;
using System.Collections.Generic;

namespace DHDM
{
	public class FocusedControlsChangedEventArgs: EventArgs
	{
		public FocusedControlsChangedEventArgs(List<StatBox> active, List<StatBox> deactivated)
		{
			Deactivated = deactivated;
			Active = active;
		}

		public List<StatBox> Active { get; private set; }
		public List<StatBox> Deactivated { get; private set; }
	}
}
