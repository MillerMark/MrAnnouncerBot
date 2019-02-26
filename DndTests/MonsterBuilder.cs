using DndCore;
using System;

namespace DndTests
{
	public static class MonsterBuilder
	{
		public static Monster BuildVioletFungus()
		{
			Monster violetFungus = new Monster();
			violetFungus.name = "Bonnie";
			violetFungus.raceClass = "Violet Fungus";
			violetFungus.creatureSize = CreatureSize.Medium;
			violetFungus.alignment = "unaligned";
			violetFungus.kind = CreatureKinds.Plants;
			violetFungus.armorClass = 5;
			violetFungus.hitPoints = 18;
			violetFungus.hitPointsDice = "4d8";
			violetFungus.speed = 5;

			violetFungus.SetAbilities(3, -4, 1, -5, 10, 0, 1, -5, 3, -4, 1, -5);
			violetFungus.conditionImmunities = Conditions.Blinded | Conditions.Deafened | Conditions.Frightened;
			violetFungus.blindsightRadius = 30;
			violetFungus.passivePerception = 6;
			violetFungus.challengeRating = 1 / 4;
			violetFungus.experiencePoints = 50;
			violetFungus.AddAttack(Attack.Melee("Rotting Touch", +2, 10, 1).AddDamage(DamageType.Necrotic, "1d8", AttackKind.NonMagical));
			violetFungus.AddMultiAttack("Rotting Touch");
			violetFungus.multiAttackCount = MultiAttackCount.d4x1;
			violetFungus.traits.Add("False Appearance. While the violet fungus remains motionless, it is indistinguishable from an ordinary fungus.");
			return violetFungus;
		}

		public static Monster BuildVineBlight()
		{
			Monster vineBlight = new Monster();
			vineBlight.name = "Joe";
			vineBlight.raceClass = "Vine Blight";
			vineBlight.creatureSize = CreatureSize.Medium;
			vineBlight.alignment = "Neutral Evil";
			vineBlight.kind = CreatureKinds.Plants;
			vineBlight.armorClass = 12;
			vineBlight.naturalArmor = true;
			vineBlight.hitPoints = 26;
			vineBlight.hitPointsDice = "4d8+8";
			vineBlight.speed = 10;

			vineBlight.SetAbilitiesFromStr(@"STR
																			15 (+2)
																			DEX
																			8 (-1)
																			CON
																			14 (+2)
																			INT
																			5 (-3)
																			WIS
																			10 (+0)
																			CHA
																			3 (-4)
																			");
			vineBlight.skillsModStealth = +1;
			vineBlight.conditionImmunities = Conditions.Blinded | Conditions.Deafened;
			vineBlight.blindsightRadius = 60;
			vineBlight.passivePerception = 10;
			vineBlight.AddLanguages(Languages.Common);
			vineBlight.challengeRating = 1 / 2;
			vineBlight.experiencePoints = 100;

			vineBlight.AddAttack(Attack.Melee("Constrict", +4, 10, 1)
				.AddDamage(DamageType.Bludgeoning, "2d6+2", AttackKind.NonMagical)
				.AddFilteredCondition(Conditions.Grappled, 12 /* escapeDC */, ComparisonFilterOption.TargetSizeLessThan, CreatureSize.Huge, 1 /* concurrentTargets */));

			vineBlight.AddAttack(Attack.Area("Entangling Plants", 15)
				.AddRecharge(RechargeOdds.TwoInSix)
				.AddDuration(DndTimeSpan.OneMinute)
				.AddFilteredCondition(Conditions.Restrained, 12)
				.AddSavingThrow(12, Ability.Strength));

			vineBlight.traits.Add("False Appearance. While the blight remains motionless, it is indistinguishable from a tangle of vines.");
			return vineBlight;

		}

		public static Monster BuildVrock()
		{
			Monster vrock = new Monster();
			vrock.name = "Clyde";
			vrock.raceClass = "Vrock";
			vrock.creatureSize = CreatureSize.Large;
			vrock.alignment = "Chaotic Evil";
			vrock.kind = CreatureKinds.Fiends;
			vrock.armorClass = 15;
			vrock.naturalArmor = true;
			vrock.hitPoints = 104;
			vrock.hitPointsDice = "11d10+44";
			vrock.speed = 40;
			vrock.flyingSpeed = 60;
			vrock.SetAbilities(17, +3, 15, +2, 18, +4, 8, -1, 13, +1, 8, -1);
			vrock.savingDexterityMod = +5;
			vrock.savingWisdomMod = +4;
			vrock.savingCharismaMod = +2;
			vrock.advantages = Against.spellsAndMagicalEffects;
			vrock.AddDamageResistance(DamageType.Cold | DamageType.Fire | DamageType.Lightning | DamageType.Bludgeoning | DamageType.Piercing | DamageType.Slashing, AttackKind.NonMagical);
			vrock.AddDamageImmunity(DamageType.Poison);
			vrock.conditionImmunities = Conditions.Poisoned;
			vrock.darkvisionRadius = 120;
			vrock.passivePerception = 11;
			vrock.AddLanguages(Languages.Abyssal);
			vrock.telepathyRadius = 120;
			vrock.challengeRating = 6;
			vrock.experiencePoints = 2300;
			vrock.AddAttack(Attack.Melee("Beak", +6, 5, 1).AddDamage(DamageType.Piercing, "2d6+3", AttackKind.NonMagical));

			vrock.AddAttack(Attack.Melee("Talons", +6, 5, 1).AddDamage(DamageType.Slashing, "2d10+3", AttackKind.NonMagical));

			Attack sporesAttack = Attack.Area("Spores", 15).AddDamage(DamageType.Poison, "1d10", AttackKind.NonMagical, TimePoint.StartOfTurn, TimePoint.EndOfTurn);
			sporesAttack.description = "A 15­-foot­-radius cloud of toxic spores extends out from the vrock. The spores spread around corners. Each creature in that area must succeed on a DC 14 Constitution saving throw or become poisoned. While poisoned in this way, a target takes 5 (1d10) poison damage at the start of each of its turns. A target can repeat the saving throw at the end of each of its turns, ending the effect on itself on a success. Emptying a vial of holy water on the target also ends the effect on it.";
			sporesAttack.conditions = Conditions.Poisoned;
			//sporesAttack.releaseTrigger = new ReleaseTrigger("Target receives splashes of holy water.");
			sporesAttack.AddSavingThrow(14, Ability.Constitution);
			vrock.AddAttack(sporesAttack.AddRecharge(RechargeOdds.OneInSix));

			Attack screech = Attack.Area("Stunning Screech", 20);
			screech.conditions = Conditions.Stunned;
			//screech.releaseTrigger = new EndOfAttackersNextTurn(1);
			screech.AddSavingThrow(14, Ability.Constitution);
			screech.includeTargetSenses = Senses.Hearing;
			// TODO: Add test to determine if an attack hits a player using screech.includeTargetSenses.
			screech.excludeCreatures = CreatureKinds.Fiends;
			screech.recharges = DndTimeSpan.FromDays(1);
			vrock.AddAttack(screech);

			vrock.AddMultiAttack("Beak", "Talons");

			vrock.traits.Add("Magic Resistance. The vrock has advantage on saving throws against spells and other magical effects.");

			return vrock;
		}
	}
}
