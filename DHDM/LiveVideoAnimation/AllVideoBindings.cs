using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DHDM
{
	public static class AllVideoBindings
	{
		static List<VideoAnimationBinding> allVideoBindings;

		static void LoadAllVideoBindings()
		{
			allVideoBindings = GoogleSheets.Get<VideoAnimationBinding>();
		}

		public static List<VideoAnimationBinding> GetAll(string sceneName)
		{
			return AllBindings.FindAll(x => x.SceneName == sceneName);
		}

		public static void Invalidate()
		{
			allVideoBindings = null;
		}

		public static VideoAnimationBinding Get(string movementFileName)
		{
			return AllBindings.FirstOrDefault(x => x.MovementFileName == movementFileName);
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
