using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GoogleHelper;

namespace CardMaker
{
	
	[SheetName(Constants.SheetName_DeckData)]
	[TabName("Cards")]
	public class Card : BaseNameId
	{
		string stylePath;
		double textLineHeight;
		double textFontSize;
		TextFontScale fontScale;
		string text;
		Deck parentDeck;
		public Deck ParentDeck
		{
			get => parentDeck; set
			{
				if (parentDeck == value)
					return;
				parentDeck = value;
				OwningTracker = parentDeck;
			}
		}

		private const double lineHeightFactor = 50d / 58d;


		[Column]
		public TextFontScale FontScale
		{
			get => fontScale;
			set
			{
				if (fontScale == value)
					return;
				fontScale = value;
				OnPropertyChanged();
				switch (fontScale)
				{
					case TextFontScale.Small:
						TextFontSize = 42;
						break;
					case TextFontScale.Normal:
						TextFontSize = 58;
						break;
					case TextFontScale.Large:
						TextFontSize = 96;
						break;
				}
				TextLineHeight = TextFontSize * lineHeightFactor;
			}
		}


		public double TextFontSize
		{
			get => textFontSize;
			set
			{
				if (textFontSize == value)
					return;
				textFontSize = value;
				OnPropertyChanged();
			}
		}


		public double TextLineHeight
		{
			get => textLineHeight;
			set
			{
				if (textLineHeight == value)
					return;
				textLineHeight = value;
				OnPropertyChanged();
			}
		}


		string parentDeckId = string.Empty;

		[Column]
		public string DeckId
		{
			get
			{
				return ParentDeck != null ? ParentDeck.ID : parentDeckId;
			}
			set
			{
				if (parentDeckId == value)
					return;
				parentDeckId = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public string Text
		{
			get => text;
			set
			{
				if (text == value)
					return;
				text = value;
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

		public ObservableCollection<Field> Fields { get; set; } = new ObservableCollection<Field>();

		[Column]
		
		public string StylePath
		{
			get => stylePath;
			set
			{
				if (stylePath == value)
					return;
				stylePath = value;
				OnPropertyChanged();
			}
		}
		

		public override string ToString()
		{
			return Name;
		}

		public void AddField(Field field)
		{
			Fields.Add(field);
			IsDirty = true;
		}

		public void RemoveField(Field field)
		{
			Fields?.Remove(field);
			field.ParentCard = null;
			field.CardId = string.Empty;
			IsDirty = true;
		}

		public Card()
		{
		}

		public Card(Deck deck)
		{
			CreateNewId();
			AddToDeck(deck);
		}

		public void AddToDeck(Deck deck)
		{
			ParentDeck = deck;
			if (ParentDeck != null)
				ParentDeck.AddCard(this);
		}
	}
}
