using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GoogleHelper;

namespace CardMaker
{
	[SheetName(Constants.SheetName_DeckData)]
	[TabName("Cards")]
	public class Card : INotifyPropertyChanged
	{
		public Deck ParentDeck { get; set; }

		string parentDeckName = string.Empty;

		public event PropertyChangedEventHandler PropertyChanged;

		bool isDirty;
		public bool IsDirty
		{
			get => isDirty; 
			set
			{
				if (isDirty == value)
					return;
				isDirty = value;
				OnPropertyChanged();
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (propertyName != "IsDirty")
				IsDirty = true;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[Indexer]
		[Column]
		public string Deck
		{
			get
			{
				return ParentDeck != null ? ParentDeck.Name : parentDeckName;
			}
			set
			{
				if (parentDeckName == value)
					return;
				parentDeckName = value;
				OnPropertyChanged();
			}
		}


		string name;
		[Indexer]
		[Column]
		public string Name
		{
			get => name;
			set
			{
				if (name == value)
					return;
				name = value;
				OnPropertyChanged();
			}
		}

		string description;
		[Column]
		public string Description
		{
			get => description;
			set
			{
				if (description == value)
					return;
				description = value;
				OnPropertyChanged();
			}
		}

		string additionalInstructions;
		[Column]
		public string AdditionalInstructions
		{
			get => additionalInstructions;
			set
			{
				if (additionalInstructions == value)
					return;
				additionalInstructions = value;
				OnPropertyChanged();
			}
		}

		Rarity rarity;
		[Column]
		public Rarity Rarity
		{
			get => rarity;
			set
			{
				if (rarity == value)
					return;
				rarity = value;
				OnPropertyChanged();
			}
		}

		ProbabilityWithinRarity probabilityWithinRarity;
		[Column]
		public ProbabilityWithinRarity ProbabilityWithinRarity
		{
			get => probabilityWithinRarity;
			set
			{
				if (probabilityWithinRarity == value)
					return;
				probabilityWithinRarity = value;
				OnPropertyChanged();
			}
		}

		CardExpires expires;
		[Column]
		public CardExpires Expires
		{
			get => expires;
			set
			{
				if (expires == value)
					return;
				expires = value;
				OnPropertyChanged();
			}
		}

		int fragmentsRequired;
		[Column]
		public int FragmentsRequired
		{
			get => fragmentsRequired;
			set
			{
				if (fragmentsRequired == value)
					return;
				fragmentsRequired = value;
				OnPropertyChanged();
			}
		}

		int cooldown;
		[Column]
		public int Cooldown
		{
			get => cooldown;
			set
			{
				if (cooldown == value)
					return;
				cooldown = value;
				OnPropertyChanged();
			}
		}

		CooldownUnits cooldownUnits = CooldownUnits.Minutes;
		[Column]
		public CooldownUnits CooldownUnits
		{
			get => cooldownUnits;
			set
			{
				if (cooldownUnits == value)
					return;
				cooldownUnits = value;
				OnPropertyChanged();
			}
		}


		bool available;
		[Column]
		public bool Available
		{
			get => available;
			set
			{
				if (available == value)
					return;
				available = value;
				OnPropertyChanged();
			}
		}

		bool finalizeLater;
		[Column]
		public bool FinalizeLater
		{
			get => finalizeLater;
			set
			{
				if (finalizeLater == value)
					return;
				finalizeLater = value;
				OnPropertyChanged();
			}
		}

		bool imageGenerated;
		[Column]
		public bool ImageGenerated
		{
			get => imageGenerated;
			set
			{
				if (imageGenerated == value)
					return;
				imageGenerated = value;
				OnPropertyChanged();
			}
		}

		bool uploaded;
		[Column]
		public bool Uploaded
		{
			get => uploaded;
			set
			{
				if (uploaded == value)
					return;
				uploaded = value;
				OnPropertyChanged();
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public Card()
		{
		}

		public Card(Deck deck)
		{
			ParentDeck = deck;
			if (ParentDeck != null)
				ParentDeck.AddCard(this);
		}
	}
}
