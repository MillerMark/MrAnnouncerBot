using System;
using System.Linq;

namespace DndCore.Enums
{
	/* 
	 * Sources where these languages became available to players:
			PHB = Player's Handbook
			MM = Monsters Manual
			MToF = Mordenkainen's Tome of Foes
			SCAG = Swords Coast Adventurer's Guide
			VGtM = Volo's Guide to Monsters
			GGtR = Guildmasters' Guide to Ravnica */

	public enum Languages
	{
		None = 0,
		// Standard Languages:
		Common = 1, // (PHB)
		Dwarvish = 2, // (PHB)
		Elvish = 4, // (PHB)
		Giant = 8, // (PHB)
		Gnomish = 16, // (PHB)
		Goblin = 32, // (PHB)
		Halfling = 64, // (PHB)
		Orc = 128, // (PHB)

		// Standard Languages (Ravnica):
		Abyssal = 256, // (PHB)
		Celestial = 512, // (PHB)
										 // Common, // (PHB)
		Draconic = 1024, // (PHB)
										 // Elvish, // (PHB)
										 // Giant, // (PHB)
		Kraul = 2048, // (GGtR)
		Loxodon = 4096, // (GGtR)
		Merfolk = 8192, // (GGtR)
		Minotaur = 16384, // (GGtR)
		Sphinx = 32768, // (MM)
		Vedalkin = 65536, // (GGtR)

		// Exotic Languages:
		//Abyssal, // (PHB)
		//Celestial, // (PHB)
		//Draconic, // (PHB)
		DeepSpeech = 131072, // (PHB)
		Infernal = 262144, // (PHB)
		Primodial = 524288, // (PHB)
		Aquan = 1048576,
		Auran = 2097152,
		Ignan = 4194304,
		Terran = 8388608,
		Sylvan = 16777216, // (PHB)
		Undercommon = 33554432, // (PHB)

		// Race/Class-Specific Languages:
		Aarakocra = 67108864, // (VGtM)
		Druidic = 134217728, // (PHB)
		Gith = 268435456, // (MToF)
		ThievesCant = 536870912, // (PHB)
														 /* 
																 // Forgotten Realms Human Languages (requires GM permission):
														 // (All in SCAG)

														 Dambrathan,
														 Bedine,
														 Alzhedo,
														 Chondathan,
														 Damaran,
														 Waelan,
														 Guran,
														 Halruaan,
														 Illuskan,
														 Roushoum,
														 Chessentan,
														 Mulhorandi,
														 Untheric,
														 Thayan,
														 Rashemi,
														 Shaaran,
														 Shou,
														 Tuigan,
														 Turmic,
														 Uluik,
														 // Monstrous Languages (available via Favored Enemy or Training):
														 BlinkDog, // (MM)
														 Bullywug, // (MM)
														 GiantEagle, // (MM)
														 GiantElk, // (MM)
														 GiantOwl, // (MM)
														 Gnoll, // (MM)
														 Grell, // (MM)
														 Grung, // (VGtM)
														 HookHorror, // (MM)
														 Kruthik, // (MToF)
														 Modron, // (MM)
														 Otyugh, // (MM)
														 Sahuagin, // (MM)
														 Slaad, // (MM)
														 //Sphinx, // (MM)
														 Thri_kreen, // (MM)
														 Tlincalli, // (VGtM)
														 Troglodyte, // (MM)
														 UmberHulk, // (MM)
														 Vegepygmy, // (VGtM)
														 WinterWolf, // (MM)
														 Worg, // (MM)
														 Yeti, // (MM)
												 */
	}
}
