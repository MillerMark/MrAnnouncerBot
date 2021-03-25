using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public interface IDungeonMasterApp
	{
		DndGame Game { get; }
		void ToggleTarget(int targetNum);
		void TogglePlayerTarget(string playerName);
		void ChangePlayerStateCommand(string command, string data);
		void ToggleCondition(string data, string condition);
		void ToggleInGameCreature(int targetNum);
		void SelectPreviousInGameCreature();
		void SelectNextInGameCreature();
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
		void InstantDiceRolledByTargets(DiceRollType diceRollType, string dieStr);
		void ChangeWealth(List<int> playerIds, decimal deltaAmount);
		void RollAttack();
		void SetClock(int hours, int minutes, int seconds);
		void AdvanceClock(int hours, int minutes, int seconds, bool resting);
		void AdvanceDate(int days, int months, int years, bool resting);
		void RollDice();
		void HideScroll();
		void DropWindup();
		void GetSavingThrowStats();
		void BreakConcentration(int playerId);
		void SetModifier(int modifier);
		void RollSave(int diceId);
		void PlayScene(string sceneName, int returnMs = -1);
		void PlaySound(string soundFileName);
		void MoveFred(string movement);
		void Speak(int playerId, string message);
		void ExecuteCommand(DungeonMasterCommand dungeonMasterCommand);
		void TellDungeonMaster(string message, bool isDetail = false);
		void TellViewers(string message);
		void NextTurn();
		void ReStackConditions();
		void PrepareSkillCheck(string skillCheck);
		void PrepareTargetSkillCheck(string skillCheck);
		void PrepareSavingThrow(string savingThrow);
		void PrepareTargetSavingThrow(string savingThrow);
		void ClearAllConditions(string targetName);
		void ApplyToTargetedCreatures(string applyCommand);
		void ApplyToOnScreenCreatures(string applyCommand, int value);
		void ApplyToSelectedCreature(string applyCommand, int value);
		void ClearPlayerDice();
		void MoveTarget(string targetingCommand);
		void SpellScrollsToggle();
		void InGameUICommand(string command);
		void TriggerHandFx(HandFxDto handFxDto);
		void ShowBackground(string sourceName);
		void ShowForeground(string sourceName);
		void ShowWeather(string weatherKeyword);
		void LaunchHandTrackingEffect(string launchCommand, string dataValue);
		void NpcScrollsToggle();
		void CardCommand(CardCommandType cardCommandType, int creatureId, string cardId = "");
		void SetNextStampedeRoll(string cardName, string userName, string damageStr, string guid);
		void StampedeNow();
		void ChangeScrollPage(string scrollPage);
		void ShowFilter(string sourceName, string filterName, bool visible);
		Character GetPlayerFromId(int playerId);
		void SetDmMood(string moodName);
		void Contest(string contest);
	}
}