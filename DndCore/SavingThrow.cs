using System;
using System.Linq;

namespace DndCore
{
	public class SavingThrow
	{
		public SavingThrow(int success, Ability ability)
		{
			Ability = ability;
			Success = success;
		}

		public int Success { get; private set; }
		public Ability Ability { get; set; }

		public bool Saves(int savingThrow)
		{
			return savingThrow >= Success;
		}
	}
}
