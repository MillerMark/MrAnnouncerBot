using System;
using System.Linq;

namespace DndCore
{
	public class TargetCountEventArgs : EventArgs
	{
		public int Count { get; set; }
		public TargetStatus TargetStatus { get; set; }
		public TargetCountEventArgs()
		{

		}
	}
}
