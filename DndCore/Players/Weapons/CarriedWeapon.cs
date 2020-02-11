using System;
using System.Linq;

namespace DndCore
{
	public class CarriedWeapon
	{
		public string Name { get; set; }
		public int Count { get; set; }
		public int HitPlusModifier { get; set; }
		public string DamageDieBonus { get; set; }
		public Weapon Weapon { get; set; }
		public string WeaponHue { get; set; }
		public string Hue1 { get; set; }
		public string Hue2 { get; set; }
		public string Hue3 { get; set; }
		public CarriedWeapon()
		{

		}
	}
}