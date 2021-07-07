//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;
using Newtonsoft.Json;

namespace DHDM
{
	public static class CreatureManager
	{
		public static void UpdatePlayerStatsInGame()
		{
			PlayerStatManager.LatestCommand = "Update";
			HubtasticBaseStation.ChangePlayerStats(JsonConvert.SerializeObject(PlayerStatManager.GetDto()));
		}


		public static void UpdateInGameCreatures()
		{
			HubtasticBaseStation.UpdateInGameCreatures("Update", AllInGameCreatures.Creatures.Where(x => x.OnScreen).ToList());
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
			{
				inGameCreature.PercentDamageJustInflicted = 0;
				inGameCreature.PercentHealthJustGiven = 0;
			}
		}

		public static void SetCreatureTarget(Creature creature, bool isTargeted)
		{
			bool stateChanged = false;
			InGameCreature inGameCreature = AllInGameCreatures.GetByCreature(creature);
			if (inGameCreature != null)
			{
				if (inGameCreature.IsTargeted != isTargeted)
				{
					stateChanged = true;
					inGameCreature.IsTargeted = isTargeted;
					UpdateInGameCreatures();
				}
			}
			else
			{
				CreatureStats playerStats = PlayerStatManager.GetPlayerStats(creature.IntId);
				if (playerStats != null)
				{
					if (playerStats.IsTargeted != isTargeted)
					{
						stateChanged = true;
						playerStats.IsTargeted = isTargeted;
						UpdatePlayerStatsInGame();
					}
				}
			}

			if (stateChanged)
				if (isTargeted)
					creature.ShowState($"Targeted!", "#A00000", "#000000");
				else
					creature.ShowState($"Not Targeted!", "#0000c0", "#000000");
		}

		public static List<Creature> GetAllPlayingCreatures()
		{
			List<Creature> result = new List<Creature>();
			result.AddRange(AllInGameCreatures.Creatures.Where(x => x.OnScreen).Select(x => x.Creature));
			result.AddRange(AllPlayers.GetActive());
			return result;
		}

		public static Creature GetCreatureFromTaleSpireId(string taleSpireId, DndCore.WhatSide whatSide = DndCore.WhatSide.All)
		{
			Creature creature = AllInGameCreatures.GetByTaleSpireId(taleSpireId, whatSide);
			if (creature != null)
				return creature;

			if (whatSide.HasFlag(DndCore.WhatSide.Friendly))
				return AllPlayers.GetFromTaleSpireId(taleSpireId);

			return null;
		}
	}
}


