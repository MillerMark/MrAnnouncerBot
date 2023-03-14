//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;
using ObsControl;

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

		public void SetMood(string moodName)
		{
			foreach (DmMood dmMood in AllDmMoods)
				if (dmMood.Keyword == moodName)
				{
					ObsManager.SetSceneItemEnabled(dmMood.Background, STR_DungeonMasterScene, true);
					ObsManager.SetSceneItemEnabled(dmMood.Foreground, STR_DungeonMasterScene, true);
				}

			foreach (DmMood dmMood in AllDmMoods)
				if (dmMood.Keyword != moodName)
				{
					DndObsManager.SetSourceVisibility(dmMood.Background, STR_DungeonMasterScene, false, 0.2);
					DndObsManager.SetSourceVisibility(dmMood.Foreground, STR_DungeonMasterScene, false, 0.2);
				}
		}
		public void Invalidate()
		{
			allDmMoods = null;
		}
	}
}
