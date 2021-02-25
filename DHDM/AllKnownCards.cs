//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using Streamloots;
using GoogleHelper;

namespace DHDM
{
	public static class AllKnownCards
	{
		private static List<T> LoadData<T>() where T : new()
		{
			return GoogleSheets.Get<T>();
		}

		public static RedemptionEventsDto Get(string cardId)
		{
			return Cards.FirstOrDefault(x => x.ID == cardId);
		}

		static void Load()
		{
			allKnownCards = LoadData<RedemptionEventsDto>();
		}
		static List<RedemptionEventsDto> allKnownCards;

		public static void Invalidate()
		{
			allKnownCards = null;
		}

		public static RedemptionEventsDto Get(CardDto cardDto)
		{
			string cardId = GetCardId(cardDto.Card); // Yeah, I know. But trust me. This is the only way to round trip our CardDto.
			return Get(cardId);
		}

		public static string GetCardId(StreamlootsCard card)
		{
			return card.soundUrl;
		}

		public static List<RedemptionEventsDto> Cards
		{
			get
			{
				if (allKnownCards == null)
					Load();
				return allKnownCards;
			}
		}

	}
}
