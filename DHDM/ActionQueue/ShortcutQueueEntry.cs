//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class ShortcutQueueEntry : ActionQueueEntry
	{
		public string ShortcutName { get; set; }
		public bool RollImmediately { get; set; }
		public ShortcutQueueEntry()
		{

		}
	}
}
