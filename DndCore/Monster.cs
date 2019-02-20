using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public class Monster : Creature
	{
		public double challengeRating;
		public double experiencePoints;

		public double strengthMod;
		public double dexterityMod;
		public double constitutionMod;
		public double intelligenceMod;
		public double wisdomMod;
		public double charismaMod;

		public double savingStrengthMod;
		public double savingDexterityMod;
		public double savingConstitutionMod;
		public double savingIntelligenceMod;
		public double savingWisdomMod;
		public double savingCharismaMod;

		public int skillsModStealth;

		public bool naturalArmor;
		public string hitPointsDice;
		public int passivePerception;
		public List<Attack> attacks = new List<Attack>();
		public List<Attack> multiAttack = new List<Attack>();
		public MultiAttackCount multiAttackCount = MultiAttackCount.oneEach;
		public List<string> traits = new List<string>();

		public void SetAbilities(int strength, int strengthMod, int dexterity, int dexterityMod, int constitution, int constitutionMod, int intelligence, int intelligenceMod, int wisdom, int wisdomMod, int charisma, int charismaMod)
		{
			this.strength = strength;
			this.strengthMod = strengthMod;
			this.dexterity = dexterity;
			this.dexterityMod = dexterityMod;
			this.constitution = constitution;
			this.constitutionMod = constitutionMod;
			this.intelligence = intelligence;
			this.intelligenceMod = intelligenceMod;
			this.wisdom = wisdom;
			this.wisdomMod = wisdomMod;
			this.charisma = charisma;
			this.charismaMod = charismaMod;
		}

		public void AddAttack(Attack attack)
		{
			attacks.Add(attack);
		}

		public void AddMultiAttack(params string[] attackNames)
		{
			foreach (string attackName in attackNames)
			{
				Attack attack = attacks.Find(x => x.Name == attackName);
				if (attack != null)
					multiAttack.Add(attack);
			}
		}

		void SetAbilityFromStr(string str, ref double ability, ref double abilityMod)
		{
			int openParen = str.IndexOf("(");
			int closeParen = str.IndexOf(")");
			string abilityStr = str.Substring(0, openParen).Trim();
			string abilityModStr = str.Substring(openParen + 1, closeParen - openParen - 1).Trim();
			ability = abilityStr.ToDouble();
			abilityMod = abilityModStr.ToDouble();
		}

		// ![](36C54F2C5F48AF603559DA1AFB31DAF9.png;https://www.dndbeyond.com/monsters/vine-blight )
		/// <summary>
		/// Sets abilities from a string copied from dndbeyond.com
		/// Expecting the string to look like this:
		///     "STR
		///      15 (+2)
		///      DEX
		///      8 (-1)
		///      CON
		///      14 (+2)
		///      INT
		///      5 (-3)
		///      WIS
		///      10 (+0)
		///      CHA
		///      3 (-4)"
		/// </summary>
		/// <param name="str"></param>
		public void SetAbilitiesFromStr(string str)
		{
			string[] lines = str.Split('\n');
			SetAbilityFromStr(lines[1], ref strength, ref strengthMod);
			SetAbilityFromStr(lines[3], ref dexterity, ref dexterityMod);
			SetAbilityFromStr(lines[5], ref constitution, ref constitutionMod);
			SetAbilityFromStr(lines[7], ref intelligence, ref intelligenceMod);
			SetAbilityFromStr(lines[9], ref wisdom, ref wisdomMod);
			SetAbilityFromStr(lines[11], ref charisma, ref charismaMod);
		}

		public void AddLanguages(Languages languages)
		{
			languagesSpoken |= languages;
			languagesUnderstood |= languages;
		}
		public DamageResult Attack(Creature creature, string attackName, int savingThrow)
		{
			Attack attack = attacks.Find(x => x.Name == attackName);
			if (attack != null)
				return attack.GetDamageResult(creature, savingThrow);
			return null;
		}
	}
}