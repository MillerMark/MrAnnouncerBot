using System;
using System.Linq;

namespace DndCore
{
	public class SoundEffect : Effect
	{
		public string soundFileName;

		public SoundEffect()
		{
			soundFileName = string.Empty;
			effectKind = EffectKind.SoundEffect;
		}

		public SoundEffect(string soundFileName)
		{
			this.soundFileName = soundFileName;
			effectKind = EffectKind.SoundEffect;
		}

		public SoundEffect(string soundFileName, int timeOffset) : this(soundFileName)
		{
			timeOffsetMs = timeOffset;
		}
	}
}
