using System;
using System.Collections.Generic;
using System.Linq;

namespace DHDM
{
	public class Attack
	{
		public string description;
		public int reach;
		public int range;
		public int rangeMax;
		public int plusToHit;
		public int numTargets = 1;
		public List<Mod> mods = new List<Mod>();
		public List<Damage> damages = new List<Damage>();
		public List<Damage> successfulSaveDamages = new List<Damage>();
		public SavingThrow savingThrow;
		Attack(string name)
		{
			Name = name;
		}

		public Damage AddDamage(DamageType damageType, string damageRoll)
		{
			Damage damage = new Damage(damageType, damageRoll);
			damages.Add(damage);
			return damage;
		}

		public string Name { get; set; }
	}
}
