using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public interface IDungeonMasterApp
	{
		void ApplyDamageHealthChange(DamageHealthChange damageHealthChange);
		string SetHiddenThreshold(int hiddenThreshold);
		int GetPlayerIdFromNameStart(string characterName);
		void RollWildMagicCheck();
		void RollWildMagic();
		void SelectCharacter(int playerId);
		void RollSkillCheck(Skills skill);
		void SetClock(int hours, int minutes, int seconds);
		void AdvanceClock(int hours, int minutes, int seconds);
		void RollDice(string diceStr, DiceRollType diceRollType);
		void HideScroll();
		void Speak(int playerId, string message);
	}
}
