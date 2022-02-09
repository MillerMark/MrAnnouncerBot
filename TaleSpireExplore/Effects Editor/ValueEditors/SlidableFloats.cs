using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace TaleSpireExplore
{
	public static class SlidableFloats
	{
		static List<SlidableFloat> allSlidableFloats = null;

		static SlidableFloats() => GoogleSheets.RegisterDocumentID("TaleSpire UI Controls", "1kik_yixNgjFVgdpG2Re3sLV4KZpMId9UEc49kwMpQ3E");

		public static void Invalidate()
		{
			allSlidableFloats = null;
		}

		public static List<SlidableFloat> GetAll()
		{
			if (allSlidableFloats == null)
				allSlidableFloats = GoogleSheets.Get<SlidableFloat>();

			return allSlidableFloats;
		}
	}
}
