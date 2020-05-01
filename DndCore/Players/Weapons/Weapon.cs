using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class Weapon : ItemViewModel
	{
		public List<Attack> attacks = new List<Attack>();
		public double normalRange = 0;  // Used if weaponProperties includes WeaponProperties.range.
		public double disadvantageRange = 0;    // Used if weaponProperties includes WeaponProperties.range.
		public WeaponProperties weaponProperties = WeaponProperties.None;
		public string damageOneHanded = string.Empty;
		public string damageTwoHanded = string.Empty;
		public string AmmunitionKind{ get; set; }
		public static Weapon From(WeaponDto weaponDto)
		{
			Weapon weapon = new Weapon();
			weapon.normalRange = MathUtils.GetInt(weaponDto.Range);
			weapon.disadvantageRange = MathUtils.GetInt(weaponDto.DisadvantageRange);
			weapon.weaponProperties = GetWeaponProperties(weaponDto);
			weapon.costValue = DndUtils.GetGoldPieces(weaponDto.Cost);
			weapon.damageOneHanded = weaponDto.DamageOneHanded;
			weapon.damageTwoHanded = weaponDto.DamageTwoHanded;
			weapon.Name = weaponDto.Name;
			weapon.AmmunitionKind = weaponDto.Ammo;
			return weapon;
		}

		static WeaponProperties GetWeaponProperties(WeaponDto weaponDto)
		{
			WeaponProperties result = WeaponProperties.None;
			if (weaponDto.Ammo.HasSomething())
				result |= WeaponProperties.Ammunition;
			if (MathUtils.IsChecked(weaponDto.Finesse))
				result |= WeaponProperties.Finesse;
			if (MathUtils.IsChecked(weaponDto.Heavy))
				result |= WeaponProperties.Heavy;
			if (MathUtils.IsChecked(weaponDto.Light))
				result |= WeaponProperties.Light;
			if (MathUtils.IsChecked(weaponDto.Martial))
				result |= WeaponProperties.Martial;
			if (MathUtils.IsChecked(weaponDto.Melee))
				result |= WeaponProperties.Melee;
			if (MathUtils.IsChecked(weaponDto.Ranged))
				result |= WeaponProperties.Ranged;
			if (MathUtils.IsChecked(weaponDto.Reach))
				result |= WeaponProperties.Reach;
			if (MathUtils.IsChecked(weaponDto.Special))
				result |= WeaponProperties.Special;
			if (MathUtils.IsChecked(weaponDto.Thrown))
				result |= WeaponProperties.Thrown;
			if (MathUtils.IsChecked(weaponDto.TwoHanded))
				result |= WeaponProperties.TwoHanded;
			if (MathUtils.IsChecked(weaponDto.Versatile))
				result |= WeaponProperties.Versatile;

			return result;

		}

		public bool RequiresAmmunition()
		{
			return weaponProperties.HasFlag(WeaponProperties.Ammunition);
		}

		public static Weapon buildBlowgun()
		{
			Weapon blowGun = new Weapon();
			blowGun.Name = "Blowgun";
			blowGun.normalRange = 25;
			blowGun.disadvantageRange = 100;
			blowGun.costValue = 10;
			blowGun.attacks.Add(new Attack(AttackNames.BlowDart).AddDamage(DamageType.Piercing, "1", AttackKind.NonMagical));
			blowGun.weaponProperties = WeaponProperties.Ammunition | WeaponProperties.Loading | WeaponProperties.Ranged;
			blowGun.weight = 1;
			return blowGun;
		}

		public static Weapon buildMagicalShortSword()
		{
			Weapon shortSword = new Weapon();
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
