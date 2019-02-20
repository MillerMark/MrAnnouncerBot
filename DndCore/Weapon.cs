using System;
using System.Linq;

namespace DndCore
{
	public class Weapon : Item
	{
		public AttackType weaponType = AttackType.none;
		public WeaponProperties weaponProperties = WeaponProperties.none;

		public double normalRange = 0;  // Used if weaponProperties includes WeaponProperties.range.
		public double longRange = 0;    // Used if weaponProperties includes WeaponProperties.range.


		public static Weapon buildShortSword()
		{
			Weapon shortSword = new Weapon();
			shortSword.weaponType = AttackType.melee;
			shortSword.name = "Shortsword";
			shortSword.costValue = 10;
			shortSword.attacks.Add(new Attack("Stab").AddDamage(DamageType.Piercing, "1d6", AttackKind.NonMagical));
			shortSword.weaponProperties = WeaponProperties.finesse | WeaponProperties.light;
			shortSword.weight = 2;
			return shortSword;
		}

		public static Weapon buildMagicalShortSword()
		{
			Weapon shortSword = new Weapon();
			shortSword.weaponType = AttackType.melee;
			shortSword.magic = true;
			shortSword.name = "Magical Shortsword";
			shortSword.costValue = 10;
			shortSword.attacks.Add(new Attack("Stab").AddDamage(DamageType.Piercing, "1d6", AttackKind.Magical));
			shortSword.weaponProperties = WeaponProperties.finesse | WeaponProperties.light;
			shortSword.weight = 2;
			return shortSword;
		}

		public static Weapon buildBlowgun()
		{
			Weapon blowGun = new Weapon();
			blowGun.weaponType = AttackType.martialRange;
			blowGun.name = "Blowgun";
			blowGun.normalRange = 25;
			blowGun.longRange = 100;
			blowGun.costValue = 10;
			blowGun.attacks.Add(new Attack("Blow Dart").AddDamage(DamageType.Piercing, "1", AttackKind.NonMagical));
			blowGun.weaponProperties = WeaponProperties.ammunition | WeaponProperties.loading | WeaponProperties.range;
			blowGun.weight = 1;
			return blowGun;
		}
		public void Attack(string attackName, Character player)
		{
			foreach (Attack attack in attacks)
			{
				if (attack.Name == attackName)
					attack.ApplyDamageTo(player);
			}
		}
	}
}
