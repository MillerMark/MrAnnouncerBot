using Unity.Physics;
using Bounce.TaleSpire.AssetManagement;
using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Board
		{
			public static bool IsLoaded
			{
				get
				{
					return (CameraController.HasInstance &&
									BoardSessionManager.HasInstance &&
									BoardSessionManager.HasBoardAndIsInNominalState &&
									!BoardSessionManager.IsLoading);
				}
			}

			// TODO: On game shutdown, dispose of legalPlacement
			static Creature.LegalPlacement legalPlacement;
			static CollisionFilter tileAndMatQuery;

			static Board()
			{
				legalPlacement = new Creature.LegalPlacement();
			}

			public static Vector3 GetFloorPositionClosestTo(Vector3 position)
			{
				const float baseRadius = 0.5f;
				legalPlacement.Init(position);
				legalPlacement.UpdateLegalCheck(position + new Vector3(0.1f, baseRadius + 0.05f, 0.1f), baseRadius * 0.65f, baseRadius * 0.8f);
				legalPlacement.UpdateLegalCheck(position + new Vector3(-0.1f, baseRadius + 0.05f, -0.1f), baseRadius * 0.65f, baseRadius * 0.8f);
				legalPlacement.UpdateLegalCheck(position + new Vector3(0f, baseRadius + 0.05f, 0f), baseRadius * 0.65f, baseRadius * 0.8f);
				return legalPlacement.GetMostCurrentLegal();
			}

			/// <summary>
			/// Creates a new creature of the specified boardAssetId, at the specified position, and returns it's CreaturedId.
			/// </summary>
			/// <param name="boardAssetId">The board asset to create.</param>
			/// <param name="position">The position to place the new asset.</param>
			/// <returns>The CreatureId as a string.</returns>
			public static string InstantiateCreature(string boardAssetId, Vector3 position)
			{
				CreatureDataV1 creatureDataV1 = new CreatureDataV1(new Bounce.Unmanaged.NGuid(boardAssetId));
				CreatureDataV2 creatureDataV2 = new CreatureDataV2(creatureDataV1);   // IsCreated is set to true in this call. Seems necessary.
				creatureDataV2.CreatureId = new CreatureGuid(new Bounce.Unmanaged.NGuid(Guid.NewGuid()));
				CreatureManager.CreateAndAddNewCreature(creatureDataV2, position, Quaternion.identity);
				BuildingBoardTool.RecordInBuildHistory(creatureDataV2.GetActiveBoardAssetId());
				return creatureDataV2.CreatureId.Value.ToString();
			}
		}
	}
}