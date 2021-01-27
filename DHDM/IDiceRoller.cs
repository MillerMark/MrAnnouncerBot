using System;
using DndCore;

namespace DHDM
{
	public interface IDiceRoller
	{
		void RollTheDice(DiceRoll diceRoll, int delayMs = 0);
		void ClearViewerDice();
		void ClearPlayerDice();
	}
}
