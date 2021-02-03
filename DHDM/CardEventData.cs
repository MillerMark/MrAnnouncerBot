//#define profiling
using System;
using System.Linq;
using GoogleHelper;

namespace DHDM
{
	[SheetName("D&D Deck Data")]
	[TabName("Cards")]
	public class CardEventData
	{
		[Indexer]
		[Column]
		public string ID { get; set; }

		[Column]
		public string Name { get; set; }
		[Column]
		public string CardReceived { get; set; }
		[Column]
		public string CardPlayed { get; set; }

		public CardEventData()
		{

		}
	}
}
