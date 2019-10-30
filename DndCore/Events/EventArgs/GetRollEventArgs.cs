using System;

namespace DndCore
{
	public class GetRollEventArgs : EventArgs
	{
		public GetRollEventArgs(string rollName)
		{
			RollName = rollName;
		}

		public string RollName { get; set; }
		public int Result { get; set; }
	}
}
