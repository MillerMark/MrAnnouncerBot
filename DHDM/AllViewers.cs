using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DHDM
{
	public static class AllViewers
	{
		// TODO: Remove RandomlyReplaceDragonHumpersViewerWithOthers (or set to false)- only for testing.
		public static bool RandomlyReplaceDragonHumpersViewerWithOthers = true;
		public static List<DndViewer> KnownViewers { get; set; }
		static AllViewers()
		{
			GoogleSheets.RegisterSpreadsheetID("D&D Viewers", "1tTZJy3WNGzc-KGGFEb-416naWlTi6QEs2bqVqPJ7pac");
			LoadViewers();
		}

		private static void LoadViewers()
		{
			KnownViewers = GoogleSheets.Get<DndViewer>();
			foreach (DndViewer dndViewer in KnownViewers)
				dndViewer.IsDirty = false;
		}

		public static void Save()
		{
			GoogleSheets.SaveChanges(KnownViewers.ToArray());
		}
		public static DndViewer Get(string userName)
		{
			DndViewer foundUser;
			if (RandomlyReplaceDragonHumpersViewerWithOthers && userName == "dragonhumpers")
				foundUser = KnownViewers[new Random().Next(KnownViewers.Count)];  // Select a random user.
			else
				foundUser = KnownViewers.FirstOrDefault(x => x.UserName == userName);

			if (foundUser == null)
			{
				foundUser = new DndViewer(userName);
				KnownViewers.Add(foundUser);
			}
			return foundUser;
		}
		public static void Invalidate()
		{
			LoadViewers();
		}
	}
}
