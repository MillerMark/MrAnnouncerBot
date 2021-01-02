using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class CardDto
	{
		public StreamlootsPurchase Purchase { get; set; }
		public StreamlootsCard Card { get; set; }
		public string Command { get; set; }
		public List<StreamlootsHand> Hands { get; set; }
		public CardDto(StreamlootsCard card)
		{
			Card = card;
			Command = "ShowCard";
		}
		public CardDto(StreamlootsPurchase purchase)
		{
			Purchase = purchase;
			Command = "ShowPurchase";
		}
		public CardDto()
		{

		}
	}
}
