using System;
using System.Linq;

namespace DHDM
{
	public class DebugLine
	{
		public string Line { get; set; }
		public bool BreakAtStart { get; set; }
		public DebugLine(string line, bool breakAtStart)
		{
			Line = line;
			BreakAtStart = breakAtStart;
		}
		public DebugLine(string line)
		{
			Line = line;
		}
		public DebugLine()
		{

		}
	}
}
