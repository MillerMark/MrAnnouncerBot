using System;
using System.Collections.Generic;
using System.Linq;

namespace DHDM
{
	public class CurseBlessingDisease
	{
		public string name;
		public string description;
		public List<Mod> mods = new List<Mod>();
		public List<ReleaseTrigger> releaseTriggers = new List<ReleaseTrigger>();

		public DndTimeSpan duration = DndTimeSpan.Infinity;
		CurseBlessingDisease()
		{

		}
	}
}
