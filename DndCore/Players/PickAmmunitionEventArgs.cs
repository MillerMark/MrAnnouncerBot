using System;
using System.Linq;

namespace DndCore
{
	public class PickAmmunitionEventArgs : EventArgs
	{
		public CarriedAmmunition Ammunition { get; set; }
		public Character Player { get; set; }
		public string AmmunitionKind { get; set; }
		public PickAmmunitionEventArgs(Character player, string ammunitionKind)
		{
			AmmunitionKind = ammunitionKind;
			Player = player;
		}
	}
}