using System;
using System.Linq;

namespace DndCore
{
	public class PickWeaponEventArgs : EventArgs
	{
		public CarriedWeapon Weapon { get; set; }
		public Creature Player { get; set; }
		public string WeaponFilter { get; set; }
		public PickWeaponEventArgs(Creature player, string weaponFilter)
		{
			WeaponFilter = weaponFilter;
			Player = player;
		}
	}
}