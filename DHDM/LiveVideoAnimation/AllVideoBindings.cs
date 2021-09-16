using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DHDM
{
	public static class AllVideoBindings
	{
		static List<VideoAnimationBinding> allVideoBindings;

		static void LoadAllVideoBindings()
		{
			allVideoBindings = GoogleSheets.Get<VideoAnimationBinding>();
		}

		public static VideoAnimationBinding Get(string sceneName)
		{
			return AllBindings.FirstOrDefault(x => x.SceneName == sceneName);
		}

		public static List<VideoAnimationBinding> AllBindings
		{
			get
			{
				if (allVideoBindings == null)
					LoadAllVideoBindings();
				return allVideoBindings;
			}
			set => allVideoBindings = value;
		}
	}
}
