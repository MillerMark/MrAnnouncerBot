using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Menus
		{
			public static Creature GetCreatureAtMenu()
			{
				CreatureMenuBoardTool creatureMenuBoardTool = SingletonBehaviour<BoardToolManager>.Instance.GetTool<CreatureMenuBoardTool>();
				if (creatureMenuBoardTool != null)
					return ReflectionHelper.GetNonPublicField<Creature>(creatureMenuBoardTool, "_selectedCreature"); ;
				return null;
			}

			public static ActionTimeline GetKnockdownStatusEmote()
			{
				List<ActionTimeline> statusEmotes = GetStatusEmotes();
				if (statusEmotes == null)
					return null;

				foreach (ActionTimeline actionTimeline in statusEmotes)
					if (actionTimeline.name == AnimationNames.KnockDown)
						return actionTimeline;
					else
					{
						//Log.Debug($"actionTimeline.name = \"{actionTimeline.name}\", DisplayName = \"{actionTimeline.DisplayName}\"");
					}

				return null;
			}

			public static List<ActionTimeline> GetStatusEmotes()
			{
				CreatureMenuBoardTool creatureMenuBoardTool = SingletonBehaviour<BoardToolManager>.Instance.GetTool<CreatureMenuBoardTool>();
				if (creatureMenuBoardTool == null)
					return new List<ActionTimeline>();

				return ReflectionHelper.GetNonPublicField<List<ActionTimeline>>(creatureMenuBoardTool, "_statusEmotes");
			}
		}
	}
}