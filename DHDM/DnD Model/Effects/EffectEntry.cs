using System;
using System.Linq;

namespace DHDM
{
	public class EffectEntry
	{
		public string Name
		{
			get { return name; }
			set
			{
				name = value;
			}
		}

		public EmitterEffect EmitterEffect { get => emitterEffect; set => emitterEffect = value; }
		public AnimationEffect AnimationEffect { get => animationEffect; set => animationEffect = value; }
		public SoundEffect SoundEffect { get => soundEffect; set => soundEffect = value; }
		public EffectKind EffectKind { get => effectKind; set => effectKind = value; }

		string name;

		EmitterEffect emitterEffect;
		AnimationEffect animationEffect;
		SoundEffect soundEffect;
		EffectKind effectKind;

		public EffectEntry(EffectKind effectKind, string name)
		{
			this.Name = name;
			this.effectKind = effectKind;
			emitterEffect = new EmitterEffect()
			{
			};
			animationEffect = new AnimationEffect()
			{
			};
			soundEffect = new SoundEffect()
			{
			};
		}
	}
}
