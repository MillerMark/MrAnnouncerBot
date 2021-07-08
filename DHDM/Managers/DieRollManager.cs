//#define profiling
using DndCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleSpireCore;

namespace DHDM
{
	// TODO: Bring all of MainWindow's die rolling code over here.
	public static class DieRollManager
	{
		public static DndGame Game { get; private set; }

		public static void Initialize(DndGame game)
		{
			Game = game;
			RollSavingThrowsForAllTargetsFunction.SavingThrowForTargetsRequested += SavingThrowForTargetsRequested;
		}

		static void BeforePlayerRolls(int playerId, DiceRoll diceRoll, ref VantageKind vantageKind)
		{
			Character player = Game.GetPlayerFromId(playerId);
			if (player == null)
				return;
			player.BeforePlayerRollsDice(diceRoll, ref vantageKind);
		}

		static void LastChanceToModifyDiceBeforeRoll(DiceRoll diceRoll)
		{
			AddVantageDice(diceRoll);
			ScaleDiceToMatchPlayers(diceRoll);
		}

		static void AddVantageDice(DiceRoll diceRoll)
		{
			VantageKind vantageKind = diceRoll.VantageKind;
			if (diceRoll.IsOnePlayer)
			{
				List<int> creatureIds = diceRoll.GetCreatureIds();
				if (creatureIds != null && creatureIds.Count == 1)
				{

					diceRoll.VantageKind = vantageKind;
					foreach (PlayerRollOptions playerRollOptions in diceRoll.PlayerRollOptions)
						if (playerRollOptions.PlayerID == creatureIds[0])
						{
							vantageKind = playerRollOptions.VantageKind;
							BeforePlayerRolls(creatureIds[0], diceRoll, ref vantageKind);
							playerRollOptions.VantageKind = vantageKind;
							break;
						}
				}
			}
			else
			{
				foreach (PlayerRollOptions playerRollOptions in diceRoll.PlayerRollOptions)
				{
					vantageKind = playerRollOptions.VantageKind;
					BeforePlayerRolls(playerRollOptions.PlayerID, diceRoll, ref vantageKind);
				}
			}
		}

		static void ScaleDiceToMatchPlayers(DiceRoll diceRoll)
		{
			if (diceRoll.IsOnePlayer)
			{
				List<int> creatureIds = diceRoll.GetCreatureIds();
				if (creatureIds != null && creatureIds.Count == 1 && creatureIds[0] >= 0)
					diceRoll.OverallDieScale = GetPlayerDieScaleById(creatureIds[0]);
			}
			else
			{
				foreach (DiceDto diceDto in diceRoll.DiceDtos)
				{
					if (diceDto?.CreatureId >= 0)
						diceDto.Scale = GetPlayerDieScaleById(diceDto.CreatureId);
				}

				foreach (PlayerRollOptions playerRollOptions in diceRoll.PlayerRollOptions)
				{
					playerRollOptions.Scale = GetPlayerDieScaleById(playerRollOptions.PlayerID);
				}
			}
		}

		public static double GetPlayerDieScaleById(int playerID)
		{
			Character player = Game.GetPlayerFromId(playerID);
			if (player != null)
			{
				double dieScale = player.GetStateDouble("_dieScale");
				if (!double.IsNaN(dieScale))
					return dieScale;
			}
			return 1;
		}

		public static DateTime LastDieRollTime { get; private set; }

		public static void SeriouslyRollTheDice(DiceRoll diceRoll)
		{
			CardEventManager.ConditionRoll(diceRoll);
			LastChanceToModifyDiceBeforeRoll(diceRoll);
			LastDieRollTime = DateTime.Now;

			if (SpellManager.nextSpellIdWeAreCasting != null)
			{
				diceRoll.SpellID = SpellManager.nextSpellIdWeAreCasting;
				SpellManager.nextSpellIdWeAreCasting = null;
			}
			SpellManager.activeSpellName = null;
			string serializedObject = JsonConvert.SerializeObject(diceRoll);
			HubtasticBaseStation.RollDice(serializedObject);
			TargetManager.ClearTargetingAfterRoll();
		}

		public static void RollingDiceNow(DiceRoll diceRoll)
		{
			HubtasticBaseStation.ClearWindup("Weapon.*!");
			HubtasticBaseStation.ClearWindup("Windup.*!");

			SystemVariables.DiceRoll = diceRoll;
		}

		public static bool LastRollIsOlderThan(int ageSeconds)
		{
			return DateTime.Now - LastDieRollTime > TimeSpan.FromSeconds(ageSeconds);
		}

		private static void SavingThrowForTargetsRequested(object sender, SavingThrowRollEventArgs ea)
		{
			// TODO: Get all the targets.
			if (Targeting.ActualKind.HasFlag(TargetKind.Volume))
			{
				CharacterPositions allTargetsInVolume = TargetManager.GetAllCreaturesInVolume();
				if (allTargetsInVolume == null)
					return;

				DiceRoll diceRoll = new DiceRoll(DiceRollType.SavingThrow, VantageKind.Normal, ea.DamageDieStr);
				diceRoll.SavingThrow = ea.Ability;

				diceRoll.SuppressLegacyRoll = true;
				diceRoll.Conditions = ea.Condition;
				if (ea.CastedSpell?.SpellCaster is Character spellCaster)
					diceRoll.HiddenThreshold = spellCaster.SpellSaveDC;

				foreach (CharacterPosition characterPosition in allTargetsInVolume.Characters)
				{
					Creature creature = CreatureManager.GetCreatureFromTaleSpireId(characterPosition.ID);
					DiceDto diceDto = null;
					if (creature is Character player)
					{
						diceDto = DiceDto.AddD20ForCharacter(player, $"{player.Name}'s Save", player.GetSavingThrowModifier(ea.Ability), DieCountsAs.savingThrow);
					}
					else
					{
						InGameCreature inGameCreature = AllInGameCreatures.GetByCreature(creature);
						if (inGameCreature != null)
							diceDto = DiceDto.D20FromInGameCreature(inGameCreature, DiceRollType.SavingThrow, diceRoll.SavingThrow);
					}
					if (diceDto == null)
					{
						continue;
					}

					diceRoll.DiceDtos.Add(diceDto);
					diceRoll.SpellID = ea.CastedSpell.ID;    // Add ea.SpellGuid so we can undo the effect after the spell dispels.
				}
				SeriouslyRollTheDice(diceRoll);
			}
		}
	}
}


