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

		public static CharacterPositions GetAllCreaturesInVolume()
		{
			VectorDto volumeCenter = Targeting.TargetPoint.ToVectorDto();
			string shapeName = Targeting.ExpectedTargetDetails.Shape.ToString();
			CharacterPositions allTargetsInVolume = TaleSpireClient.GetAllTargetsInVolume(volumeCenter, shapeName, Targeting.ExpectedTargetDetails.Dimensions);
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
		public static void AboutToRoll()
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

		public static List<Creature> GetTargets()
		{
			List<Creature> creatures = new List<Creature>();
			foreach (CreatureStats creatureStats in PlayerStatManager.Players)
				if (creatureStats.IsTargeted)
					creatures.Add(AllPlayers.GetFromId(creatureStats.CreatureId));

			creatures.AddRange(AllInGameCreatures.GetTargeted());

			return creatures;
		}


	}
}


