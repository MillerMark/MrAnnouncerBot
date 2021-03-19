//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DHDM
{
	public class DmMoodManager
	{
		const string STR_DungeonMasterScene = ">>>DungeonMaster<<<";
		List<DmMood> allDmMoods;
		public List<DmMood> AllDmMoods
		{
			get
			{
				if (allDmMoods == null)
					allDmMoods = GoogleSheets.Get<DmMood>();
				return allDmMoods;
			}
			set => allDmMoods = value;
		}
		public ObsManager ObsManager { get; set; }
		public DmMoodManager(ObsManager obsManager)
		{
			ObsManager = obsManager;
		}

		public void SetMood(string moodName)
		{
			foreach (DmMood dmMood in AllDmMoods)
				if (dmMood.Keyword == moodName)
				{
					ObsManager.SetSourceVisibility(dmMood.Background, STR_DungeonMasterScene, true);
					ObsManager.SetSourceVisibility(dmMood.Foreground, STR_DungeonMasterScene, true);
				}

			foreach (DmMood dmMood in AllDmMoods)
				if (dmMood.Keyword != moodName)
				{
					ObsManager.SetSourceVisibility(dmMood.Background, STR_DungeonMasterScene, false, 0.2);
					ObsManager.SetSourceVisibility(dmMood.Foreground, STR_DungeonMasterScene, false, 0.2);
				}
		}
		public void Invalidate()
		{
			allDmMoods = null;
		}
	}
}
