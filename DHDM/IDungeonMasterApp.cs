using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public interface IDungeonMasterApp
	{
		void ToggleTarget(int targetNum);
		void TogglePlayerTarget(string playerName);
		void ChangePlayerStateCommand(string command, string data);
		void ToggleCondition(string data, string condition);
		void ToggleInGameCreature(int targetNum);
		void TalkInGameCreature(int targetNum);
		void TargetCommand(string command);
		void Apply(string command, decimal value, List<int> playerIds);
		string GetPlayFirstNameFromId(int playerId);
		void SetBoolProperty(int playerId, string propertyName, bool value);
		bool GetBoolProperty(int playerId, string propertyName);
		void SetSaveHiddenThreshold(int hiddenThreshold);
		int GetPlayerIdFromName(string characterName);
		int GetActivePlayerId();
		int GetLastSpellSave();
		void SetPlayerVolume(string mainFolder, int newVolume);
		void SetPlayerFolder(string mainFolder, string newTheme);
		void StopPlayer(string mainFolder);
		void ApplyDamageHealthChange(DamageHealthChange damageHealthChange);
		void RollWildMagicCheck();
		void RollWildMagic();
		void SelectCharacter(int playerId);
		void SelectPlayerShortcut(string shortcutName, int playerId);
		void GetData(string dataId);
		void SetVantage(VantageKind dataId, int playerId);
		void RollSkillCheck(Skills skill, List<int> playerIds);
		void RollSavingThrow(Ability ability, List<int> playerIds);
		void InstantDice(DiceRollType diceRollType, string dieStr, List<int> playerIds);
		void ChangeWealth(List<int> playerIds, decimal deltaAmount);
		void RollAttack();
		void SetClock(int hours, int minutes, int seconds);
		void AdvanceClock(int hours, int minutes, int seconds);
		void AdvanceDate(int days, int months, int years);
		void RollDice();
		void HideScroll();
		void DropWindup();
		void GetSavingThrowStats();
		void BreakConcentration(int playerId);
		void SetModifier(int modifier);
		void RollSave(int diceId);
		void PlayScene(string sceneName);
		void PlaySound(string soundFileName);
		void MoveFred(string movement);
		void Speak(int playerId, string message);
		void ExecuteCommand(DungeonMasterCommand dungeonMasterCommand);
		void TellDungeonMaster(string message, bool isDetail = false);
		void TellViewers(string message);
		void NextTurn();
		void ReStackConditions();
		void PrepareSkillCheck(string skillCheck);
		void PrepareSavingThrow(string savingThrow);
		void ClearAllConditions(string targetName);
		void ApplyToTargetedCreatures(string applyCommand);
		void ClearDice();
	}
}
