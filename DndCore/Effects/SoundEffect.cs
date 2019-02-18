using System;
using System.Linq;

namespace DndCore
{
	public class SoundEffect : Effect
	{
		public string soundFileName;

		public SoundEffect()
		{
			this.soundFileName = string.Empty;
		}

		public SoundEffect(string soundFileName)
		{
			this.soundFileName = soundFileName;
		}
	}
}
