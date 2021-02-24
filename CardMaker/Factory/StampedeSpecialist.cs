using System;
using System.Linq;
using GoogleHelper;

namespace CardMaker
{
	[SheetName("D&D Deck Data")]
	[TabName("StampedeCards")]
	public class StampedeSpecialist
	{
		[Column]
		[Indexer]
		public string ImageLayerName { get; set; }

		[Column]
		[Indexer]
		public string DiceDamageStr { get; set; }

		[Column]
		public string CardName { get; set; }

		[Column]
		public string Description { get; set; }

		[Column]
		public string AlertMessage { get; set; }

		[Column]
		public string CardPlayedMessage { get; set; }

		public StampedeSpecialist()
		{

		}
	}
}

