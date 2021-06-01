using System;
using System.Linq;
using GoogleHelper;

namespace TaleSpireExplore
{
	[SheetName("TaleSpire Effects")]
	[TabName("Effects")]
	public class TaleSpireEffect
	{
		[Column]
		[Indexer]
		public string Name { get; set; }

		[Column]
		public string Effect { get; set; }
		
		public TaleSpireEffect()
		{

		}
	}
}
