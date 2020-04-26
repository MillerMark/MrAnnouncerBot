using System;
using System.Linq;

namespace DHDM
{
	public class SoundCommand
	{
		public SoundCommandType type { get; set; }
		public string strData { get; set; }
		public string mainFolder { get; set; }
		public double numericData { get; set; }
		public SoundCommand(string mainFolder = "")
		{
			this.mainFolder = mainFolder;
		}
	}
}
