using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DHDM
{
	public static class AllViewers
	{
		// TODO: Remove RandomlyReplaceDragonHumpersViewerWithOthers (or set to false)- only for testing.
		public static bool RandomlyReplaceDragonHumpersViewerWithOthers = false;
		public static List<DndViewer> KnownViewers { get; set; }
		static AllViewers()
		{
			GoogleSheets.RegisterSpreadsheetID("D&D Viewers", "1tTZJy3WNGzc-KGGFEb-416naWlTi6QEs2bqVqPJ7pac");
			//LoadViewers();
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

		/// <summary>
		/// Adds the specified charge to the specified viewer and returns the total charge count for the specified chargeName.
		/// </summary>
		/// <param name="viewerName">The name of the viewer to add the charges to.</param>
		/// <param name="chargeName">The name of the charge to add.</param>
		/// <param name="chargeCount">The number of charges to add.</param>
		/// <returns>The total charge count for the specified chargeName.</returns>
		public static int AddCharge(string viewerName, string chargeName, int chargeCount)
		{
			if (DndViewer.TestingSayAnything)
				viewerName = "SayAnythingTester";
			DndViewer viewer = Get(viewerName);
			return viewer.AddCharge(chargeName, chargeCount);
		}
	}
}
