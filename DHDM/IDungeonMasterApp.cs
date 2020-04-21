using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public interface IDungeonMasterApp
	{
		string GetPlayFirstNameFromId(int playerId);
		void SetBoolProperty(int playerId, string propertyName, bool value);
		bool GetBoolProperty(int playerId, string propertyName);
		void SetHiddenThreshold(int hiddenThreshold);
		int GetPlayerIdFromName(string characterName);
		int GetActivePlayerId();
		int GetLastSpellSave();
		void SetThemeVolume(int newVolume);
		void SetTheme(string newTheme);
		void ApplyDamageHealthChange(DamageHealthChange damageHealthChange);
		void RollWildMagicCheck();
		void RollWildMagic();
		void SelectCharacter(int playerId);
		void SelectPlayerShortcut(string shortcutName, int playerId);
		void GetData(string dataId);
		void SetVantage(VantageKind dataId, int playerId);
		void RollSkillCheck(Skills skill, List<int> playerIds);
		void RollSavingThrow(Ability ability, List<int> playerIds);
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
		void MoveFred(string movement);
		void Speak(int playerId, string message);
		void ExecuteCommand(DungeonMasterCommand dungeonMasterCommand);
		void TellDungeonMaster(string message, bool isDetail = false);
		void TellViewers(string message);
	}
}
