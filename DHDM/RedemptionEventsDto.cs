//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DHDM
{
	[Document("D&D Deck Data")]
	[Sheet("Cards")]
	public class RedemptionEventsDto
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

		public RedemptionEventsDto()
		{

		}
	}
}
