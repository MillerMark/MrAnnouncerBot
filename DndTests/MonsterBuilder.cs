using DndCore;
using DndCore.Enums;
using DndCore.CoreClasses;
using System;

namespace DndTests
{
	public static class MonsterBuilder
	{
		public static Monster BuildVioletFungus(string name = "")
		{
			Monster violetFungus = new Monster();
			if (string.IsNullOrEmpty(name))
				violetFungus.name = "Bonnie";
			else
				violetFungus.name = name;
			
			violetFungus.raceClass = "Violet Fungus";
			violetFungus.creatureSize = CreatureSize.Medium;
			violetFungus.alignment = AlignmentNames.unaligned;
			violetFungus.kind = CreatureKinds.Plants;
			violetFungus.baseArmorClass = 5;
			violetFungus.hitPoints = 18;
			violetFungus.hitPointsDice = Dice.d8x4;
			violetFungus.baseSpeed = 5;

			violetFungus.SetAbilities(3, -4, 1, -5, 10, 0, 1, -5, 3, -4, 1, -5);
			violetFungus.conditionImmunities = Conditions.Blinded | Conditions.Deafened | Conditions.Frightened;
			violetFungus.blindsightRadius = 30;
			violetFungus.passivePerception = 6;
			violetFungus.challengeRating = 1 / 4;
			violetFungus.experiencePoints = 50;
			violetFungus.AddAttack(Attack.Melee(AttackNames.RottingTouch, +2, 10, 1).AddDamage(DamageType.Necrotic, Dice.d8x1, AttackKind.NonMagical));
			violetFungus.AddMultiAttack(AttackNames.RottingTouch);
			violetFungus.multiAttackCount = MultiAttackCount.d4x1;
			violetFungus.traits.Add("False Appearance. While the violet fungus remains motionless, it is indistinguishable from an ordinary fungus.");
			return violetFungus;
		}

		public static Monster BuildVineBlight(string name = "")
		{
			Monster vineBlight = new Monster();
			if (string.IsNullOrEmpty(name))
				vineBlight.name = "Joe";
			else
				vineBlight.name = name;
			
			vineBlight.raceClass = "Vine Blight";
			vineBlight.creatureSize = CreatureSize.Medium;
			vineBlight.alignment = AlignmentNames.NeutralEvil;
			vineBlight.kind = CreatureKinds.Plants;
			vineBlight.baseArmorClass = 12;
			vineBlight.naturalArmor = true;
			vineBlight.hitPoints = 26;
			vineBlight.hitPointsDice = Dice.d8x4.Plus(8);
			vineBlight.baseSpeed = 10;

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

			vineBlight.AddAttack(Attack.Melee(AttackNames.Constrict, +4, 10, 1)
				.AddDamage(DamageType.Bludgeoning, Dice.d6x2.Plus(2), AttackKind.NonMagical)
				.AddGrapple(12, CreatureSizes.LargeOrSmaller));

			vineBlight.AddAttack(Attack.Area(AttackNames.EntanglingPlants, 15)
				.AddRecharge(RechargeOdds.TwoInSix)
				.AddDuration(DndTimeSpan.OneMinute)
				.AddCondition(Conditions.Restrained, 12, Ability.Strength));

			vineBlight.traits.Add("False Appearance. While the blight remains motionless, it is indistinguishable from a tangle of vines.");
			return vineBlight;

		}

		public static Monster BuildVrock(string name = "")
		{
			Monster vrock = new Monster();
			if (string.IsNullOrEmpty(name))
				vrock.name = "Clyde";
			else
				vrock.name = name;
			
			vrock.raceClass = "Vrock";
			vrock.creatureSize = CreatureSize.Large;
			vrock.alignment = AlignmentNames.ChaoticEvil;
			vrock.kind = CreatureKinds.Fiends;
			vrock.baseArmorClass = 15;
			vrock.naturalArmor = true;
			vrock.hitPoints = 104;
			vrock.hitPointsDice = Dice.d10x11.Plus(44);;
			vrock.baseSpeed = 40;
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
			vrock.AddAttack(Attack.Melee(AttackNames.Beak, +6, 5, 1).AddDamage(DamageType.Piercing, Dice.d6x2.Plus(3), AttackKind.NonMagical));

			vrock.AddAttack(Attack.Melee(AttackNames.Talons, +6, 5, 1).AddDamage(DamageType.Slashing, Dice.d10x2.Plus(3), AttackKind.NonMagical));

			Attack sporesAttack = Attack.Area(AttackNames.Spores, 15).AddDamage(DamageType.Poison, Dice.d10x1, AttackKind.NonMagical, TimePoint.StartOfTurn, TimePoint.EndOfTurn, Conditions.Poisoned, 14, Ability.Constitution);
			sporesAttack.description = "A 15­-foot­-radius cloud of toxic spores extends out from the vrock. The spores spread around corners. Each creature in that area must succeed on a DC 14 Constitution saving throw or become poisoned. While poisoned in this way, a target takes 5 (1d10) poison damage at the start of each of its turns. A target can repeat the saving throw at the end of each of its turns, ending the effect on itself on a success. Emptying a vial of holy water on the target also ends the effect on it.";
			//sporesAttack.releaseTrigger = new ReleaseTrigger("Target receives splashes of holy water.");
			sporesAttack.AddRecharge(RechargeOdds.OneInSix);
			vrock.AddAttack(sporesAttack);

			Attack screech = Attack.Area(AttackNames.StunningScreech, 20).AddCondition(Conditions.Stunned, 14, Ability.Constitution);
			screech.LastDamage.IncludeTargetSenses = Senses.Hearing;
			// TODO: Add test to determine if an attack hits a player using screech.includeTargetSenses.
			screech.LastDamage.ExcludeCreatureKinds(CreatureKinds.Fiends);
			screech.recharges = DndTimeSpan.FromDays(1);
			vrock.AddAttack(screech);

			vrock.AddMultiAttack(AttackNames.Beak, AttackNames.Talons);

			vrock.traits.Add("Magic Resistance. The vrock has advantage on saving throws against spells and other magical effects.");

			return vrock;
		}
	}
}
