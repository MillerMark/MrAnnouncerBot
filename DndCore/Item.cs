using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DndCore
{
	public class Ammunition : Item
	{
		public static Ammunition buildBlowgunNeedlePack()
		{
			Ammunition blowDart = new Ammunition();
			blowDart.name = "Blow Needle";
			blowDart.costValue = 1;
			blowDart.weight = 1;
			blowDart.count = 50;
			return blowDart;
		}
	}

	public enum WeaponProperties
	{
		none = 0,

		ammunition = 1,  /* You can use a weapon that has the ammunition property to make a ranged 
                      attack only if you have ammunition to fire from the weapon. Each time 
                      you attack with the weapon, you expend one piece of ammunition. Drawing 
                      the ammunition from a quiver, case, or other container is part of the 
                      attack (you need a free hand to load a one-handed weapon). At the end of 
                      the battle, you can recover half your expended ammunition by taking a 
                      minute to search the battlefield. If you use a weapon that has the 
                      ammunition property to make a melee attack, you treat the weapon as an 
                      improvised weapon (see “Improvised Weapons” later in the section). A 
                      sling must be loaded to deal any damage when used in this way. */

		finesse = 2,     /* When making an attack with a finesse weapon, you use your choice of your 
                      Strength or Dexterity modifier for the attack and damage rolls. You must 
                      use the same modifier for both rolls.*/

		heavy = 4,       /* Small creatures have disadvantage on attack rolls with heavy weapons. A 
                      heavy weapon's size and bulk make it too large for a Small creature to 
                      use effectively. */

		light = 8,       /* A light weapon is small and easy to handle, making it ideal for use when 
                      fighting with two weapons. */

		loading = 16,    /* Because of the time required to load this weapon, you can fire only one 
                      piece of ammunition from it when you use an action, bonus action, or 
                      reaction to fire it, regardless of the number of attacks you can normally 
                      make. */

		range = 32,      /* A weapon that can be used to make a ranged attack has a range in parentheses 
                      after the ammunition or thrown property. The range lists two numbers. The 
                      first is the weapon's normal range in feet, and the second indicates the 
                      weapon's long range. When attacking a target beyond normal range, you have 
                      disadvantage on the attack roll. You can't attack a target beyond the weapon's 
                      long range. */

		reach = 64,      /* This weapon adds 5 feet to your reach when you attack with it, as well as 
                      when determining your reach for opportunity attacks with it. */

		special = 128,   /* A weapon with the special property has unusual rules governing its use, 
                      explained in the weapon's description. */

		thrown = 256,    /* If a weapon has the thrown property, you can throw the weapon to make a ranged 
                      attack. If the weapon is a melee weapon, you use the same ability modifier for 
                      that attack roll and damage roll that you would use for a melee attack with 
                      the weapon. For example, if you throw a handaxe, you use your Strength, but if 
                      you throw a dagger, you can use either your Strength or your Dexterity, since 
                      the dagger has the finesse property. */

		twoHanded = 512, /* This weapon requires two hands when you attack with it. */

		versatile = 1024 /* This weapon can be used with one or two hands. A damage value in parentheses 
                      appears with the property–the damage when the weapon is used with two hands to 
                      make a melee attack. */
	}


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

	public class Item
	{
		public DndTimeSpan equipTime = DndTimeSpan.Zero;
		public DndTimeSpan unequipTime = DndTimeSpan.Zero;
		public string name = string.Empty;
		public string description = string.Empty;
		public bool consumable = false;
		public int weight = 0;
		public int costValue = 0;
		public bool magic = false;
		public bool equipped = false;
		public bool silvered = false;
		public bool adamantine = false;
		public int count = 1;
		public List<Mod> mods = new List<Mod>();
		public List<Attack> attacks = new List<Attack>();
		public List<CurseBlessingDisease> cursesBlessingsDiseases = new List<CurseBlessingDisease>();
		public List<Effect> consumedEffects = new List<Effect>();
		public List<Effect> equippedEffects = new List<Effect>();
		public List<Effect> unequippedEffects = new List<Effect>();
		public List<Effect> discardedEffects = new List<Effect>();

		public void Consume(Character owner)
		{
			if (!consumable)
				return;

			foreach (Mod mod in mods)
				if (mod.requiresConsumption)
					owner.ApplyModPermanently(mod, name);
		}

		void ApplyAllMods(Character owner)
		{
			foreach (Mod mod in mods)
				if (!mod.requiresConsumption)
					if (!mod.requiresEquipped || equipped)
						owner.ApplyModTemporarily(mod, name);
		}
	}
}
