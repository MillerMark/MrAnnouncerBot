using System;
using System.ComponentModel;
using System.Linq;

namespace DndCore
{
	public enum WeaponCategories
	{
		None,

		//Simple Melee Weapons:
		Club,
		Dagger,
		Greatclub,
		Handaxe,
		Javelin,
		[Description("Light Hammer")]
		LightHammer,
		Mace,
		Quarterstaff,
		Sickle,
		Spear,
		
		//Simple Ranged Weapons:
		[Description("Crossbow, Light")]
		CrossbowLight,
		Dart,
		Shortbow,
		Sling,
		
		//Martial Melee Weapons:
		Battleaxe,
		Flail,
		Glaive,
		Greataxe,
		Greatsword,
		Halberd,
		Lance,
		Longsword,
		Maul,
		Morningstar,
		Pike,
		Rapier,
		Scimitar,
		Shortsword,
		Trident,
		[Description("War Pick")]
		WarPick,
		Warhammer,
		Whip,

		// Martial Ranged Weapons:
		Blowgun,
		[Description("Crossbow, Hand")]
		CrossbowHand,
		[Description("Crossbow, Heavy")]
		CrossbowHeavy,
		Longbow,
		Net
	}
}
