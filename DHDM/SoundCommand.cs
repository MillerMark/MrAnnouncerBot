using System;
using System.Linq;

namespace DHDM
{
	public class SoundCommand
	{
		public SoundCommandType type { get; set; }
		public string strData { get; set; }
		public double numericData { get; set; }
		public SoundCommand()
		{

		}
	}
}
