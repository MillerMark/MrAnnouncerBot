using System;
using System.Linq;

namespace CardMaker
{
	public enum CardExpires
	{
		Immediately,
		EndOfTurn,
		EndOfNextTurn,
		EndOfGame,
		Never
	}
}
