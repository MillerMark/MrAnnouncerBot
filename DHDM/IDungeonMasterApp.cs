using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public interface IDungeonMasterApp
	{
		void SetHiddenThreshold(int hiddenThreshold);
		int GetPlayerIdFromNameStart(string characterName);
		int GetActivePlayerId();
		void ApplyDamageHealthChange(DamageHealthChange damageHealthChange);
		void RollWildMagicCheck();
		void RollWildMagic();
		void SelectCharacter(int playerId);
		void SelectPlayerShortcut(string shortcutName, int playerId);
		void GetData(string dataId);
		void SetVantage(VantageKind dataId);
		void RollSkillCheck(Skills skill, List<int> playerIds);
		void RollSavingThrow(Ability ability, List<int> playerIds);
		void RollAttack();
		void SetClock(int hours, int minutes, int seconds);
		void AdvanceClock(int hours, int minutes, int seconds);
		void AdvanceDate(int days, int months, int years);
		void RollDice();
		void HideScroll();
		void DropWindup();
		void BreakConcentration(int playerId);
		void PlayScene(string sceneName);
		void MoveFred(string movement);
		void Speak(int playerId, string message);
		void ExecuteCommand(DungeonMasterCommand dungeonMasterCommand);
		void TellDungeonMaster(string message, bool isDetail = false);
		void TellViewers(string message);
	}
}
