using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DHDM
{
	public class EffectEntry : INotifyPropertyChanged
	{
		public string Name
		{
			get { return name; }
			set
			{
				if (name == value)
				{
					return;
				}

				name = value;
				OnPropertyChanged();
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
			Name = name;
			this.effectKind = effectKind;
			emitterEffect = new EmitterEffect();
			animationEffect = new AnimationEffect();
			soundEffect = new SoundEffect();
		}


		private bool isSelected;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsSelected
		{
			get => isSelected;
			set
			{
				isSelected = value;
				OnPropertyChanged();

				if (!isSelected)
					IsDisplaying = true;
			}
		}

		// Create separate IsDisplaying/IsEditing so we can use the BooleanToVisibilityConverter
		// without having to create our own (for negating).
		private bool isDisplaying = true;

		public bool IsDisplaying
		{
			get { return isDisplaying; }
			set
			{
				if (isDisplaying == value)
					return;

				isDisplaying = value;
				IsEditing = !isDisplaying;
				OnPropertyChanged();
			}
		}

		private bool isEditing;

		public bool IsEditing
		{
			get { return isEditing; }
			set
			{
				if (isEditing == value)
					return;

				isEditing = value;
				IsDisplaying = !isEditing;
				OnPropertyChanged();
			}
		}

		public bool IsAnimation
		{
			get => EffectKind == EffectKind.Animation;
			set
			{
				if (value) EffectKind = EffectKind.Animation;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Effect));
			}
		}

		public bool IsEmitter
		{
			get => EffectKind == EffectKind.SoundEffect;
			set
			{
				if (value) EffectKind = EffectKind.SoundEffect;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Effect));
			}
		}

		public bool IsSound
		{
			get => EffectKind == EffectKind.SoundEffect;
			set
			{
				if (value) EffectKind = EffectKind.SoundEffect;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Effect));
			}
		}

		public Effect Effect
		{
			get
			{
				switch (EffectKind)
				{
					case EffectKind.Animation:
						return AnimationEffect;
					case EffectKind.Emitter:
						return EmitterEffect;
					default:
						return SoundEffect;
				}
			}
			set => OnPropertyChanged(nameof(Effect));
		}

		protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
