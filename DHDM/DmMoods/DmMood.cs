﻿//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DHDM
{
	[Document("DnD")]
	[Sheet("DmMood")]
	public class DmMood
	{
		[Column]
		[Indexer]
		public string Keyword { get; set; }
		[Column]
		public string Background { get; set; }
		[Column]
		public string Foreground { get; set; }
		public DmMood()
		{

		}
	}
}
