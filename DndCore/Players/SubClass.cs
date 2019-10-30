using System;
using System.Linq;

namespace DndCore
{
	public enum SubClass
	{
		None,

		// Barbarian subclasses...
		PathOfTheAncestralGuardian,
		PathOfTheBattlerager,
		PathOfTheBerserker,
		PathOfTheStormHerald,
		PathOfTheTotemWarrior,
		PathOfTheWildSoul,
		PathOfTheZealot,

		// Bard subclasses...
		CollegeOfGlamour,
		CollegeOfLore,
		CollegeOfSatire,
		CollegeOfSwords,
		CollegeOfValor,
		CollegeOfWhispers,

		// Cleric subclasses...
		ArcanaDomain,
		CityDomain,
		DeathDomain,
		ForgeDomain,
		GraveDomain,
		KnowledgeDomain,
		LifeDomain,
		LightDomain,
		NatureDomain,
		ProtectionDomain,
		TempestDomain,
		TrickeryDomain,
		WarDomain,

		// Druid subclasses...
		CircleOfDreams,
		CircleOfTheLand,
		CircleOfTheMoon,
		CircleOfTheShepherd,
		CircleOfSpores,
		CircleOfTwilight,

		//Fighter subclasses...
		ArcaneArcher,
		Banneret_PurpleDragonKnight,
		BattleMaster,
		Brute,
		Cavalier,
		Champion,
		EldritchKnight,
		Knight,
		Samurai,
		Sharpshooter,
		StormKnight,

		//Monk subclasses...
		WayOfTheDrunkenMaster,
		WayOfTheFourElements,
		WayOfTheKensei,
		WayOfTheLongDeath,
		WayOfTheOpenHand,
		WayOfShadow,
		WayOfTheSunSoul,
		WayOfTranquility,

		//Paladin subclasses...
		OathOfTheAncients,
		OathOfConquest,
		OathOfTheCrown,
		OathOfDevotion,
		OathOfRedemption,
		OathOfTreachery,
		OathOfVengeance,
		Oathbreaker,

		//Ranger subclasses...
		BeastMaster,
		GloomStalker,
		HorizonWalker,
		Hunter,
		MonsterSlayer,
		PrimevalGuardian,

		//Rogue subclasses...
		ArcaneTrickster,
		Assassin,
		Inquisitive,
		Mastermind,
		Scout,
		Swashbuckler,
		Thief,

		//Sorcerer subclasses...
		DivineSoul,
		DraconicBloodline,
		PhoenixSorcery,
		SeaSorcery,
		ShadowMagic,
		StoneSorcery,
		StormSorcery,
		WildMagic,

		//Warlock subclasses...
		TheArchfey,
		TheCelestial,
		TheFiend,
		GhostInTheMachine,
		TheGreatOldOne,
		TheHexblade,
		TheRavenQueen,
		TheSeeker,
		TheUndying,

		//Wizard subclasses...
		Artificer,
		Bladesinging,
		LoreMastery,
		SchoolOfAbjuration,
		SchoolOfConjuration,
		SchoolOfDivination,
		SchoolOfEnchantment,
		SchoolOfEvocation,
		SchoolOfIllusion,
		SchoolOfInvention,
		SchoolOfNecromancy,
		SchoolOfTransmutation,
		Technomancy,
		Theurgy,
		WarMagic,
	}
}