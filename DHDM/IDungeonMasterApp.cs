using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public interface IDungeonMasterApp
	{
		void SetHiddenThreshold(int hiddenThreshold);
		int GetPlayerIdFromNameStart(string characterName);
		void ApplyDamageHealthChange(DamageHealthChange damageHealthChange);
		void RollWildMagicCheck();
		void RollWildMagic();
		void SelectCharacter(int playerId);
		void SelectPlayerShortcut(string shortcutName);
		void RollSkillCheck(Skills skill);
		void RollSavingThrow(Ability ability);
		void RollAttack();
		void SetClock(int hours, int minutes, int seconds);
		void AdvanceClock(int hours, int minutes, int seconds);
		void RollDice(string diceStr, DiceRollType diceRollType);
		void HideScroll();
		void DropWindup();
		void PlayScene(string sceneName);
		void Speak(int playerId, string message);
		void ExecuteCommand(DungeonMasterCommand dungeonMasterCommand);
		void TellDungeonMaster(string message, bool isDetail = false);
		void TellViewers(string message);
	}
}
