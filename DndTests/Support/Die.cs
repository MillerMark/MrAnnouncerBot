using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DndTests
{
	public static class Die
	{
		public static int Roll(int sides)
		{
			return (int)Math.Floor((double)new Random().Next(sides)) + 1;
		}

		public static int getAbilityScore()
		{
			return Roll(6) + Roll(6) + Roll(6);
		}


	}
}
