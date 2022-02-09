using System;
using System.Linq;
using SheetsPersist;

namespace TaleSpireExplore
{
	[DocumentName("TaleSpire Effects")]
	[SheetName("Effects")]
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
