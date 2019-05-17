using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public enum ReleaseAction
	{
		Wait,
		HolyWater
	}

	public class CurseBlessingDisease
	{
		public string description;
		//public List<ReleaseTrigger> releaseTriggers = new List<ReleaseTrigger>();
		public DndTimeSpan duration = DndTimeSpan.Forever;
		public List<Mod> mods = new List<Mod>();
		public string name;
		public ReleaseAction ReleaseAction = ReleaseAction.Wait;

		CurseBlessingDisease()
		{

		}
	}
}
