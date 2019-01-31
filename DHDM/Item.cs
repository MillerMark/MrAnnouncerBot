//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DHDM
//{
//	public enum TimeMeasure
//	{
//		actions,
//		seconds
//	}

//	enum ModType
//	{
//		playerProperty,
//		condition,
//		incomingAttack,
//		outgoingAttack
//	}

//	enum DamageType
//	{
//		None = 0,
//		Acid = 1,
//		Bludgeoning = 2,
//		Cold = 4,
//		Fire = 8,
//		Force = 16,
//		Lightning = 32,
//		Necrotic = 64,
//		Piercing = 128,
//		Poison = 256,
//		Psychic = 512,
//		Radiant = 1024,
//		Slashing = 2048,
//		Thunder = 4096
//	}

//	enum Conditions
//	{
//		none = 0,
//		blinded = 1,
//		charmed = 2,
//		deafened = 4,
//		fatigued = 8,
//		frightened = 16,
//		grappled = 32,
//		incapacitated = 64,
//		invisible = 128,
//		paralyzed = 256,
//		petrified = 512,
//		poisoned = 1024,
//		prone = 2048,
//		restrained = 4096,
//		stunned = 8192,
//		unconscious = 16384
//	}

//	public class Mod
//	{
//		ModType type = ModType.playerProperty;
//		string targetName;  // e.g., target property name.
//		bool requiresEquipped;
//		bool requiresConsumption;
//		DamageType damageTypeFilter = DamageType.None;
//		Conditions condition = Conditions.none;
//		int offset = 0;
//		int multiplier = 1; // < 1 for resistance, > 1 for vulnerability
//		DndTimeSpan repeats = DndTimeSpan.Zero;
//		DateTime lastApplied = DateTime.MinValue;

//		public Mod()
//		{

//		}
//	}

//	class Damage
//	{
//		// TODO: add filtering - includeFilter, excludeFilter
//		// for body sizes
//		public Damage(DamageType damageType, string damageRoll)
//		{

//		}
//	}

//	enum Ability
//	{
//		strength = 1,
//		dexterity = 2,
//		constitution = 4,
//		intelligence = 8,
//		wisdom = 16,
//		charisma = 32,
//		none = 0
//	}

//	class SavingThrow
//	{
//		public SavingThrow(int success, Ability ability) {
//			Ability = ability;
//			Success = success;
//		}

//		public int Success { get; private set; }
//		public Ability Ability { get; set; }
//	}


//	class Attack
//	{
//		string description;
//		int reach;
//		int range;
//		int rangeMax;
//		int plusToHit;
//		int numTargets = 1;
//		List<Mod> mods = new List<Mod>();
//		List<Damage> damages = new List<Damage>();
//		List<Damage> successfulSaveDamages = new List<Damage>();
//		SavingThrow savingThrow;
//		Attack(string name)
//		{
//			Name = name;
//		}

//		Damage addDamage(DamageType damageType, string damageRoll)
//		{
//			Damage damage = new Damage(damageType, damageRoll);
//			damages.Add(damage);
//			return damage;
//		}

//		public string Name { get; set; }
//	}
//}

//class Item
//{
//	DndTimeSpan EquipTime = DndTimeSpan.Zero;
//	DndTimeSpan UnequipTime = DndTimeSpan.Zero;
//	string name;
//	List<Mod> mods = new List<Mod>();
//	List<Attack> attacks = new List<Attack>();
//	List<CurseBlessingDisease> cursesBlessingsDiseases = new List<CurseBlessingDisease>();
//	bool consumable;
//	int weight = 0;
//	int costValue;
//	bool magic;
//	bool silvered;
//	bool adamantine;
//	int count = 1;
//	string description = string.Empty;
//	Effect consumedEffect;
//	Effect equippedEffect;
//	Effect unequippedEffect;
//	Effect discardedEffect;
//}
//}
