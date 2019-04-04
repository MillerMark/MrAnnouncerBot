using Newtonsoft.Json;
using System;
using DndCore;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DndUI
{
	public class EffectEntry : ListEntry, INotifyPropertyChanged
	{
		public EmitterEffect EmitterEffect { get; set; }
		public AnimationEffect AnimationEffect { get; set; }
		public SoundEffect SoundEffect { get; set; }
		public EffectKind EffectKind { get; set; }

		public EffectEntry(EffectKind effectKind, string name)
		{
			Name = name;
			EffectKind = effectKind;
			EmitterEffect = new EmitterEffect();
			AnimationEffect = new AnimationEffect();
			SoundEffect = new SoundEffect();
		}

		PlaceholderType type = PlaceholderType.None;

		public PlaceholderType PlaceholderType
		{
			get { return type; }
			set
			{
				if (type == value)
					return;
				type = value;
				OnPropertyChanged();
			}
		}

		private bool isSelected;
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


		[JsonIgnore]
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

		public Effect GetPrimaryEffect()
		{
			if (EffectKind == EffectKind.Animation)
				return AnimationEffect;
			if (EffectKind == EffectKind.Emitter)
				return EmitterEffect;
			if (EffectKind == EffectKind.SoundEffect)
				return SoundEffect;
			if (EffectKind == EffectKind.Placeholder)
				return new PlaceholderEffect(Name, PlaceholderType);

			return null;
		}
	}
}
