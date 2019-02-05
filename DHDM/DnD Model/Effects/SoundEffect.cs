using System;
using System.Linq;

namespace DHDM
{
	public class SoundEffect : Effect
	{
		public string soundFileName;

		public SoundEffect(string soundFileName)
		{
			this.soundFileName = soundFileName;
		}
	}
}
