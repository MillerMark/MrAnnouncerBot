using System;
using System.Linq;
using SheetsPersist;

namespace CardMaker
{
	[DocumentName("D&D Deck Data")]
	[SheetName("StampedeCards")]
	public class StampedeSpecialist
	{
		[Column]
		[Indexer]
		public string ImageLayerName { get; set; }

		[Column]
		[Indexer]
		public string TotalDamage { get; set; }

		[Column]
		public string DiceDamageStr { get; set; }

		[Column]
		public string CardName { get; set; }
		[Column]
		public string VideoName { get; set; }

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

