using System;
using System.Linq;
using System.Collections.Generic;
using DndCore.Enums;
using DndCore.ViewModels;

namespace DndCore
{
	public class Weapon : ItemViewModel
	{

		public List<Attack> attacks = new List<Attack>();
		public double longRange = 0;    // Used if weaponProperties includes WeaponProperties.range.

		public double normalRange = 0;  // Used if weaponProperties includes WeaponProperties.range.
		public WeaponCategories weaponCategories = WeaponCategories.None;
		public WeaponProperties weaponProperties = WeaponProperties.None;
		public AttackType weaponType = AttackType.None;

		public static Weapon buildBlowgun()
		{
			Weapon blowGun = new Weapon();
			blowGun.weaponType = AttackType.MartialRange;
			blowGun.Name = "Blowgun";
			blowGun.normalRange = 25;
			blowGun.longRange = 100;
			blowGun.costValue = 10;
			blowGun.attacks.Add(new Attack(AttackNames.BlowDart).AddDamage(DamageType.Piercing, "1", AttackKind.NonMagical));
			blowGun.weaponProperties = WeaponProperties.Ammunition | WeaponProperties.Loading | WeaponProperties.Range;
			blowGun.weight = 1;
			return blowGun;
		}

		public static Weapon buildMagicalShortSword()
		{
			Weapon shortSword = new Weapon();
			shortSword.weaponType = AttackType.Melee;
			shortSword.magic = true;
			shortSword.Name = "Magical Shortsword";
			shortSword.costValue = 10;
			shortSword.attacks.Add(new Attack(AttackNames.Stab).AddDamage(DamageType.Piercing, Dice.d6x1, AttackKind.Magical));
			shortSword.weaponProperties = WeaponProperties.Finesse | WeaponProperties.Light;
			shortSword.weight = 2;
			return shortSword;
		}

		public static Weapon buildShortSword()
		{
			Weapon shortSword = new Weapon();
			shortSword.weaponType = AttackType.Melee;
			shortSword.Name = "Shortsword";
			shortSword.costValue = 10;
			shortSword.attacks.Add(new Attack(AttackNames.Stab).AddDamage(DamageType.Piercing, Dice.d6x1, AttackKind.NonMagical));
			shortSword.weaponProperties = WeaponProperties.Finesse | WeaponProperties.Light;
			shortSword.weight = 2;
			return shortSword;
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
