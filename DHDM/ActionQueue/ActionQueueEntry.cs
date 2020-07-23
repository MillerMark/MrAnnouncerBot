//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public abstract class ActionQueueEntry
	{
		public int PlayerId { get; set; }
		public ActionQueueEntry()
		{

		}
	}
}
