using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public class Weapon : Item
	{
		public AttackType weaponType = AttackType.None;
		public WeaponProperties weaponProperties = WeaponProperties.None;
		public WeaponCategories weaponCategories = WeaponCategories.None;

		public double normalRange = 0;  // Used if weaponProperties includes WeaponProperties.range.
		public double longRange = 0;    // Used if weaponProperties includes WeaponProperties.range.

		public List<Attack> attacks = new List<Attack>();

		public static Weapon buildShortSword()
		{
			Weapon shortSword = new Weapon();
			shortSword.weaponType = AttackType.Melee;
			shortSword.name = "Shortsword";
			shortSword.costValue = 10;
			shortSword.attacks.Add(new Attack("Stab").AddDamage(DamageType.Piercing, "1d6", AttackKind.NonMagical));
			shortSword.weaponProperties = WeaponProperties.Finesse | WeaponProperties.Light;
			shortSword.weight = 2;
			return shortSword;
		}

		public static Weapon buildMagicalShortSword()
		{
			Weapon shortSword = new Weapon();
			shortSword.weaponType = AttackType.Melee;
			shortSword.magic = true;
			shortSword.name = "Magical Shortsword";
			shortSword.costValue = 10;
			shortSword.attacks.Add(new Attack("Stab").AddDamage(DamageType.Piercing, "1d6", AttackKind.Magical));
			shortSword.weaponProperties = WeaponProperties.Finesse | WeaponProperties.Light;
			shortSword.weight = 2;
			return shortSword;
		}

		public static Weapon buildBlowgun()
		{
			Weapon blowGun = new Weapon();
			blowGun.weaponType = AttackType.MartialRange;
			blowGun.name = "Blowgun";
			blowGun.normalRange = 25;
			blowGun.longRange = 100;
			blowGun.costValue = 10;
			blowGun.attacks.Add(new Attack("Blow Dart").AddDamage(DamageType.Piercing, "1", AttackKind.NonMagical));
			blowGun.weaponProperties = WeaponProperties.Ammunition | WeaponProperties.Loading | WeaponProperties.Range;
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
