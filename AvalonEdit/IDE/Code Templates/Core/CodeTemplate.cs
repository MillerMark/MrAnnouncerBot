//#define profiling
using System;
using System.Linq;
using SheetsPersist;

namespace SuperAvalonEdit
{
	[Document("IDE")]
	[Sheet("Templates")]
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
