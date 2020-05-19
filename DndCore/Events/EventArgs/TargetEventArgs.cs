using System;

namespace DndCore
{
	public class TargetEventArgs : EventArgs
	{
		public Target Target { get; set; }
		public Character Player { get; set; }
		public bool AllowSelf { get; set; }
		public TargetEventArgs()
		{

		}
	}
}
