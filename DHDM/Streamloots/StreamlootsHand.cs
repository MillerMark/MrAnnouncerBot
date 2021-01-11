using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class StreamlootsHand
	{
		public int CharacterId { get; set; }

		bool isShown;

		public bool IsShown
		{
			get
			{
				return isShown;
			}
			set
			{
				if (isShown == value)
					return;
				isShown = value;
				if (!isShown)
					SelectedCard = null;
			}
		}
		
		public StreamlootsCard SelectedCard { get; set; }
		public List<StreamlootsCard> Cards { get; set; } = new List<StreamlootsCard>();
		public int Count => Cards.Count;
		public int HueShift { get; set; }
		public StreamlootsHand()
		{

		}
		public void AddCard(StreamlootsCard card)
		{
			card.Guid = Guid.NewGuid().ToString();
			Cards.Add(card);
		}
		public int GetSelectedCardIndex()
		{
			if (SelectedCard == null)
				return -1;
			for (int i = 0; i < Cards.Count; i++)
				if (Cards[i] == SelectedCard)
				{
					return i;
				}
			return -1;
		}
	}
}
