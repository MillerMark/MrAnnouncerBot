using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace DndCore
{
	public class EffectEntry : ListEntry, INotifyPropertyChanged
	{

		// Create separate IsDisplaying/IsEditing so we can use the BooleanToVisibilityConverter
		// without having to create our own (for negating).
		bool isDisplaying = true;

		bool isEditing;

		bool isSelected;

		PlaceholderType type = PlaceholderType.None;

		public EffectEntry(EffectKind effectKind, string name)
		{
			Name = name;
			EffectKind = effectKind;
			EmitterEffect = new EmitterEffect();
			AnimationEffect = new AnimationEffect();
			SoundEffect = new SoundEffect();
		}

		public AnimationEffect AnimationEffect { get; set; }


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
		public EffectKind EffectKind { get; set; }
		public EmitterEffect EmitterEffect { get; set; }

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
		public SoundEffect SoundEffect { get; set; }

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
