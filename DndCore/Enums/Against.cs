using System;
using System.Linq;

namespace DndCore
{
	public enum Against
	{
		none,
		blinded = 1,
		charmed = 2,
		deafened = 4,
		fatigued = 8,
		frightened = 16,
		grappled = 32,
		incapacitated = 64,
		invisible = 128,
		paralyzed = 256,
		petrified = 512,
		poisoned = 1024,
		prone = 2048,
		restrained = 4096,
		stunned = 8192,
		unconscious = 16384,
		spells = 32768,
		magic = 65536,
		spellsAndMagicalEffects = 131072,
		effectsThatTurnsUndead = 262144,
		putToSleep = 524288,
		spellsAndMagicalEffectsThatAffectItsMind = 1048576,
		strengthChecks = 2097152,
		wisdomChecks = 4194304,
		intelligenceChecks = 8388608,
		charismaChecks = 16777216,
		constitutionChecks = 33554432,
		dexterityChecks = 67108864
	}
}
