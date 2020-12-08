using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using GoogleHelper;
using System.Windows.Media;

namespace CardMaker
{
	[SheetName(Constants.SheetName_DeckData)]
	[TabName("Cards")]
	public class Card : BaseNameId
	{
		string placeholder;
		string layerModStr;
		Brush fontBrush;
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

		AllLayers allLayers = new AllLayers();
		bool layersAreDirty;

		[Column]
		public string LayerMods
		{
			get
			{
				if (layerModStr == null || layersAreDirty)
				{
					layerModStr = CreateLayerMods();
					layersAreDirty = false;
				}
				return layerModStr;
			}
			set
			{
				if (layerModStr == value)
					return;
				layerModStr = value;
				CreateLayerDetails();
				OnPropertyChanged();
			}
		}

		void AddLayerDetail(string layerDetail)
		{
			int openParenPos = layerDetail.IndexOf('(');
			if (openParenPos < 1)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}

			string layerName = layerDetail.Substring(0, openParenPos).Trim();
			LayerDetails layer = allLayers.Get(layerName);

			string layerParameters = layerDetail.Substring(openParenPos + 1).Trim().TrimEnd(')');

			string[] assignments = layerParameters.Split(',');
			foreach (string assignment in assignments)
				layer.AddAssignment(assignment);
		}

		void CreateLayerDetails()
		{
			allLayers.Clear();
			if (string.IsNullOrWhiteSpace(layerModStr))
				return;
			string[] parts = layerModStr.Split(';');
			foreach (string part in parts)
				AddLayerDetail(part);
		}

		string CreateLayerMods()
		{
			StringBuilder result = new StringBuilder();
			foreach (LayerDetails layerDetails in allLayers.Details)
			{
				string layerDetailsStr = layerDetails.GetStr();
				if (layerDetailsStr.Length > 0)
				{
					if (result.Length > 0)
						result.Append("; ");
					result.Append(layerDetailsStr);
				}
			}
			return result.ToString();
		}

		public void InvalidateLayerMods()
		{
			layerModStr = null;
		}


		public Brush FontBrush
		{
			get => fontBrush;
			set
			{
				if (fontBrush == value)
					return;
				fontBrush = value;
				OnPropertyChanged();
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


		bool available = true;
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
		
		[Column]
		public string Placeholder
		{
			get => placeholder;
			set
			{
				if (placeholder == value)
					return;
				placeholder = value;
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
			allLayers.PropertyChanged += AllLayers_PropertyChanged;
		}

		private void AllLayers_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (allLayers.IsDirty)
			{
				LayerPropertiesHaveChanged();
			}
		}

		public void LayerPropertiesHaveChanged()
		{
			layersAreDirty = true;
			OnPropertyChanged(nameof(LayerMods));
		}

		public Card(Deck deck) : this()
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

		public LayerDetails GetLayerDetails(string layerName)
		{
			return allLayers.Get(layerName);
		}
	}
}
