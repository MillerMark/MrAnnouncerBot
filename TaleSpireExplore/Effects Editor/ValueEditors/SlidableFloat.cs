﻿using System;
using SheetsPersist;
using System.Linq;

namespace TaleSpireExplore
{
	[Document("TaleSpire UI Controls")]
	[Sheet("SlidableFloats")]
	public class SlidableFloat
	{
		[Indexer]
		[Column]
		public string Name { get; set; }

		[Column]
		public double Min { get; set; }

		[Column]
		public double Max { get; set; }

		[Column]
		public int DecimalPlaces { get; set; }

		public bool Matches(string name, string[] allPaths)
		{
			string nameLower = Name.ToLower();
			if (nameLower == name.ToLower())
				return true;

			return allPaths.Any(x => x.EndsWith(nameLower, true, System.Globalization.CultureInfo.CurrentCulture));
		}


	}
}
