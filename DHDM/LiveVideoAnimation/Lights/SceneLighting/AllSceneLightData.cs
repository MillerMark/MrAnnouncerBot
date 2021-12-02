using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public static class AllSceneLightData
	{
		static List<SceneLightData> allLightData;

		public static List<SceneLightData> AllLightData
		{
			get
			{
				if (allLightData == null)
					allLightData = GoogleHelper.GoogleSheets.Get<SceneLightData>();

				return allLightData;
			}
		}

		public static SceneLightData Get(string sceneName)
		{
			return AllLightData.FirstOrDefault(x => x.SceneName == sceneName);
		}

		public static void Invalidate()
		{
			allLightData = null;
		}
	}
}