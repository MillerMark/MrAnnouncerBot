//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;
using TaleSpireCore;

namespace DHDM
{
	public static class TargetManager
	{
		const string STR_Favorite = "Favorite";
		static Dictionary<int, SavedTargets> savedTargets = new Dictionary<int, SavedTargets>();
		static Dictionary<string, SavedTargets> favoriteTargets = new Dictionary<string, SavedTargets>();

		static TargetManager()
		{
			TargetCreaturesInVolume.TargetCreaturesInVolumeRequest += TargetCreaturesInVolume_TargetCreaturesInVolumeRequest;
		}

		private static void TargetCreaturesInVolume_TargetCreaturesInVolumeRequest(object sender, WhatSideEventArgs ea)
		{
			if (Targeting.ActualKind.HasFlag(TargetKind.Volume))
			{
				CharacterPositions characterPositions = TaleSpireClient.GetAllCreaturesInVolume(Targeting.TargetPoint.ToVectorDto(),
					Targeting.ExpectedTargetDetails.Shape.ToString(), Targeting.ExpectedTargetDetails.DimensionsFeet,
					ea.WhatSide.ToString());

				TaleSpireClient.CleanUpTargets();
				if (characterPositions != null)
				{
					List<string> charactersToTarget = characterPositions.Characters.Select(x => x.ID).ToList();
					TaleSpireClient.TargetCreatures(charactersToTarget);
					TargetCreaturesByTaleSpireId(charactersToTarget);
				}
				CreatureManager.UpdateInGameStats();
			}

		}

		public static void TargetPoint(ApiResponse response)
		{
			VectorDto vector = response.GetData<VectorDto>();
			if (vector == null)
				return;

			Targeting.SetPoint(new Vector(vector.x, vector.y, vector.z));
		}

		public static void TargetActivePoint()
		{
			ApiResponse response = TaleSpireClient.Invoke("Target", new string[] { "Point" });
			if (response == null)
				return;
			TargetPoint(response);
			Targeting.Ready();
		}

		// TODO: Consolidate with the code in TargetCreaturesInVolume_TargetCreaturesInVolumeRequest and the WhatSide parameter
		public static CharacterPositions GetAllCreaturesInVolume()
		{
			VectorDto volumeCenter = Targeting.TargetPoint.ToVectorDto();
			string shapeName = Targeting.ExpectedTargetDetails.Shape.ToString();
			CharacterPositions allTargetsInVolume = TaleSpireClient.GetAllCreaturesInVolume(volumeCenter, shapeName, Targeting.ExpectedTargetDetails.DimensionsFeet);
			return allTargetsInVolume;
		}

		public static void ClearTargetingAfterRoll()
		{
			TaleSpireClient.TargetsAreReady();
		}

		public static void ClearTargetedInGameCreaturesInTaleSpire()
		{
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
				if (inGameCreature.IsTargeted)
					TaleSpireClient.SetTargeted(inGameCreature.TaleSpireId, false);
		}

		public static void TargetOnScreenNpcsInTaleSpire()
		{
			foreach (InGameCreature inGameCreature in AllInGameCreatures.Creatures)
				if (inGameCreature.OnScreen && !inGameCreature.IsTargeted)
				{
					inGameCreature.IsTargeted = inGameCreature.OnScreen;
					TaleSpireClient.SetTargeted(inGameCreature.TaleSpireId, true);
				}
		}

		public static void ClearPlayerTargetingInTaleSpire()
		{
			foreach (CreatureStats creatureStats in PlayerStatManager.Players)
			{
				if (creatureStats.IsTargeted)
				{
					Character player = AllPlayers.GetFromId(creatureStats.CreatureId);
					if (player != null)
						TaleSpireClient.SetTargeted(player.taleSpireId, false);
				}
			}
		}

		public static void TargetAllPlayersInTaleSpire()
		{
			foreach (CreatureStats creatureStats in PlayerStatManager.Players)
			{
				if (creatureStats.IsTargeted)
				{
					Character player = AllPlayers.GetFromId(creatureStats.CreatureId);
					if (player != null)
						TaleSpireClient.SetTargeted(player.taleSpireId, true);
				}
			}
		}

		public static void AboutToValidate()
		{
			if (!Targeting.IsReady)
			{
				// Which modes were waiting on a flashlight?
				if (Targeting.ExpectedTargetDetails.Kind.HasFlag(TargetKind.Location) ||
					Targeting.ExpectedTargetDetails.Kind.HasFlag(TargetKind.Volume))
					TargetActivePoint();
			}
			TaleSpireClient.RemoveTargetingUI();
		}

		public static void AboutToRoll()
		{
			// May not need anything here.
		}

		public static List<Creature> GetTargets()
		{
			List<Creature> creatures = new List<Creature>();
			foreach (CreatureStats creatureStats in PlayerStatManager.Players)
				if (creatureStats.IsTargeted)
					creatures.Add(AllPlayers.GetFromId(creatureStats.CreatureId));

			creatures.AddRange(AllInGameCreatures.GetTargeted());

			return creatures;
		}

		public static void Save(int creatureId)
		{
			savedTargets[creatureId] = GetCurrentTargets();
		}

		private static SavedTargets GetCurrentTargets()
		{
			SavedTargets currentTargets = new SavedTargets();
			currentTargets.TargetedCreatures = GetTargets().Select(x => x.taleSpireId).ToList();
			return currentTargets;
		}

		static void TargetCreaturesByTaleSpireId(SavedTargets savedTargets)
		{
			TargetCreaturesByTaleSpireId(savedTargets.TargetedCreatures);
		}

		private static void TargetCreaturesByTaleSpireId(List<string> targetCreatureIds)
		{
			foreach (CreatureStats creatureStats in PlayerStatManager.Players)
			{
				Character player = AllPlayers.GetFromId(creatureStats.CreatureId);
				if (player != null)
					creatureStats.IsTargeted = targetCreatureIds.Contains(player.taleSpireId);
				else
					creatureStats.IsTargeted = false;
			}

			foreach (InGameCreature creature in AllInGameCreatures.Creatures)
				creature.IsTargeted = targetCreatureIds.Contains(creature.TaleSpireId);
		}

		public static void Load(int creatureId)
		{
			if (!savedTargets.ContainsKey(creatureId))
				return;
			LoadTargets(savedTargets[creatureId]);
		}

		private static void LoadTargets(SavedTargets targets)
		{
			TaleSpireClient.CleanUpTargets();
			TaleSpireClient.RemoveTargetingUI();
			TargetCreaturesByTaleSpireId(targets);
			CreatureManager.UpdateInGameStats();
			foreach (string taleSpireId in targets.TargetedCreatures)
				TaleSpireClient.SetTargeted(taleSpireId, true);
		}

		public static void LoadOnly(int activeTurnCreatureId, int lastTurnCreatureId)
		{
			if (!savedTargets.ContainsKey(activeTurnCreatureId))
				return;

			List<string> currentlyTargetedCreatures;
			List<string> soonToBeTargetedCreatures = savedTargets[activeTurnCreatureId].TargetedCreatures;

			if (!savedTargets.ContainsKey(lastTurnCreatureId))
				currentlyTargetedCreatures = new List<string>();
			else
				currentlyTargetedCreatures = savedTargets[lastTurnCreatureId].TargetedCreatures;

			List<string> creaturesToUntarget = currentlyTargetedCreatures.Except(soonToBeTargetedCreatures).ToList();
			List<string> creaturesToTarget = soonToBeTargetedCreatures.Except(currentlyTargetedCreatures).ToList();

			TargetCreaturesByTaleSpireId(savedTargets[activeTurnCreatureId]);
			CreatureManager.UpdateInGameStats();
			foreach (string taleSpireId in creaturesToUntarget)
				TaleSpireClient.SetTargeted(taleSpireId, false);
			foreach (string taleSpireId in creaturesToTarget)
				TaleSpireClient.SetTargeted(taleSpireId, true);
		}

		public static void ClearTaleSpireTargets(int creatureId)
		{
			if (!savedTargets.ContainsKey(creatureId))
				return;
			foreach (string taleSpireId in savedTargets[creatureId].TargetedCreatures)
				TaleSpireClient.SetTargeted(taleSpireId, false);
		}

		public static void ClearTargetHistory()
		{
			savedTargets.Clear();
		}
		static void SaveTargets(string id)
		{
			favoriteTargets[id] = GetCurrentTargets();
		}

		static void LoadTargets(string id)
		{
			if (!favoriteTargets.ContainsKey(id))
				return;
			LoadTargets(favoriteTargets[id]);
		}

		public static void HandleFavoritesCommand(string targetingCommand)
		{
			int favoritePosition = targetingCommand.IndexOf(STR_Favorite);
			if (favoritePosition < 0)
				return;
			string id = targetingCommand.Substring(favoritePosition + STR_Favorite.Length);
			string loadSaveCommand = targetingCommand.Substring(0, favoritePosition);
			if (loadSaveCommand == "Save")
				SaveTargets(id);
			else if (loadSaveCommand == "Load")
				LoadTargets(id);
		}
	}
}


