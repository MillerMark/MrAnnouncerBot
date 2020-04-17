using System;
using System.Linq;

namespace DndCore
{
	public class PickWeaponEventArgs : EventArgs
	{
		public CarriedWeapon Weapon { get; set; }
		public Character Player { get; set; }
		public string WeaponFilter { get; set; }
		public PickWeaponEventArgs(Character player, string weaponFilter)
		{
			WeaponFilter = weaponFilter;
			Player = player;
		}
	}
}