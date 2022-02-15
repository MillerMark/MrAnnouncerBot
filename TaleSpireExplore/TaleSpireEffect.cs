using System;
using System.Linq;
using SheetsPersist;

namespace TaleSpireExplore
{
	[Document("TaleSpire Effects")]
	[Sheet("Effects")]
	public class TaleSpireEffect
	{
		[Column]
		[Indexer]
		public string Name { get; set; }

		[Column]
		public string Effect { get; set; }

		[Column]
		public string Category { get; set; }

		public TaleSpireEffect()
		{

		}
	}
}
