using System;
using System.Linq;

namespace DHDM
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
	}
}
