using Unity.Mathematics;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using LordAshes;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class PersistentEffects
		{
			static List<string> effectsToInitialize = new List<string>();
			const string STR_Goat1BoardAssetId = "71a127d8-a437-414d-a845-a39606a8a2fa";
			//const string STR_RichTextSizeModifier = "<size=0>";
			static readonly string STR_PersistentEffect = "$CodeRush.PersistentEffect$";
			public static void Create()
			{
				float3 pointerPos = RulerHelpers.GetPointerPos();
				string creatureId = Board.InstantiateCreature(STR_Goat1BoardAssetId, new Vector3(pointerPos.x, pointerPos.y, pointerPos.z));
				effectsToInitialize.Add(creatureId);
			}

			static List<string> updatedCreatures = new List<string>();

			static void UpdatePersistentEffect(CreatureBoardAsset creatureAsset, string value)
			{
				if (creatureAsset == null)
					return;
				Log.Warning($"UpdatePersistentEffect {creatureAsset.CreatureId} - \"{value}\"");
			}

			static void DataChangeCallback(StatMessaging.Change[] obj)
			{
				foreach (StatMessaging.Change change in obj)
					if (change.action == StatMessaging.ChangeType.added || change.action == StatMessaging.ChangeType.modified)
					{
						CreaturePresenter.TryGetAsset(change.cid, out CreatureBoardAsset creatureAsset);
						UpdatePersistentEffect(creatureAsset, change.value);
					}
			}

			static PersistentEffects()
			{
				StatMessaging.Subscribe(STR_PersistentEffect, DataChangeCallback);
			}

			public static void Update()
			{
				if (effectsToInitialize.Count == 0)
					return;

				foreach (string creatureId in effectsToInitialize)
				{
					CreaturePresenter.TryGetAsset(new CreatureGuid(creatureId), out CreatureBoardAsset creatureAsset);
					
					if (creatureAsset != null)
					{
						updatedCreatures.Add(creatureId);
						Spells.AttachEffect("MediumFire", creatureId, creatureId, 0, 0, 0, 0, 0);
						CreatureManager.SetCreatureName(creatureAsset.CreatureId, "Effect");
						StatMessaging.SetInfo(creatureAsset.CreatureId, STR_PersistentEffect, "MediumFire");
					}
					else
					{
						Log.Debug($"creatureAsset is null this update cycle....");
					}
				}

				if (updatedCreatures.Count > 0)
				{
					foreach (string creatureId in updatedCreatures)
						effectsToInitialize.Remove(creatureId);

					updatedCreatures.Clear();
				}


			}
		}
	}
}