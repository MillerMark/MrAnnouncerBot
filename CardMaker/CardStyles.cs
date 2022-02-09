using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace CardMaker
{
	public static class CardStyles
	{
		static List<SavedCardStyle> allKnownCardStyles;

		public static List<SavedCardStyle> AllKnownCardStyles
		{
			get
			{
				if (allKnownCardStyles == null)
					allKnownCardStyles = GoogleSheets.Get<SavedCardStyle>();
				return allKnownCardStyles;
			}
		}

		public static SavedCardStyle Get(string cardName)
		{
			return AllKnownCardStyles.FirstOrDefault(x => x.CardName == cardName);
		}

		public static void Save(Card card)
		{
			if (card == null)
				return;
			SavedCardStyle savedCard = Get(card.Name);
			if (savedCard == null)
			{
				savedCard = new SavedCardStyle(card);
				AllKnownCardStyles.Add(savedCard);
			}
			else
				savedCard.LoadFrom(card);

			GoogleSheets.SaveChanges(savedCard);
		}

		public static bool Apply(Card card)
		{
			if (card == null)
				return false;
			SavedCardStyle savedCard = Get(card.Name);
			if (savedCard == null)
				return false;

			savedCard.ApplyTo(card);
			return true;
		}
	}
}
