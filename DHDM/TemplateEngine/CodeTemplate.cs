//#define profiling
using System;
using System.Linq;
using GoogleHelper;

namespace DHDM
{
	[SheetName("DnD")]
	[TabName("Templates")]
	public class CodeTemplate
	{
		[Indexer]
		[Column]
		public string Template { get; set; }

		[Column]
		public string Expansion { get; set; }

		[Column]
		public string Context { get; set; }

		public CodeTemplate()
		{

		}
	}
}
