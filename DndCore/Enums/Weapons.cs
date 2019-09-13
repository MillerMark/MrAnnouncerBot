using System;
using System.Linq;

namespace DndCore
{
	public enum Weapons : Int64
	{
		None = 0,
		Battleaxe = 1,
		Blowgun = 2,
		Club = 4,
		Crossbow_Hand = 8, 		HandCrossbow = 8,
		Crossbow_Heavy = 16, 		HeavyCrossbow = 16,
		Crossbow_Light = 32, 		LightCrossbow = 32,
		Dagger = 64,
		Dart = 128,
		Flail = 256,
		Glaive = 512,
		Greataxe = 1024,
		Greatclub = 2048,
		Greatsword = 4096,
		Halberd = 8192,
		Handaxe = 16384,
		Javelin = 32768,
		Lance = 65536,
		LightHammer = 131072,
		Longbow = 262144,
		Longsword = 524288,
		Mace = 1048576,
		Maul = 2097152,
		Morningstar = 4194304,
		Net = 8388608,
		Pike = 16777216,
		Quarterstaff = 33554432,
		Rapier = 67108864,
		Scimitar = 134217728,
		Shortbow = 268435456,
		Shortsword = 536870912,
		Sickle = 1073741824,
		Sling = 2147483648,
		Spear = 4294967296,
		Trident = 8589934592,
		UnarmedStrike = 17179869184,
		WarPick = 34359738368,
		Warhammer = 68719476736,
		Whip = 137438953472,
		Wizard = Dagger | Dart | Sling | Quarterstaff | LightCrossbow,
		Simple = Club |
					Crossbow_Light |
					Dagger |
					Dart |
					Greatclub |
					Handaxe |
					Javelin |
					LightCrossbow |
					LightHammer |
					Mace |
					Quarterstaff |
					Shortbow |
					Sickle |
					Sling |
					Spear |
					UnarmedStrike,
		Martial = Battleaxe |
					Blowgun |
					Crossbow_Hand |
					Crossbow_Heavy |
					Flail |
					Glaive |
					Greataxe |
					Greatsword |
					Halberd |
					Lance |
					Longbow |
					Longsword |
					Maul |
					Morningstar |
					Net |
					Pike |
					Rapier |
					Scimitar |
					Shortsword |
					Trident |
					Warhammer |
					WarPick |
					Whip,
		All = Battleaxe |
					Blowgun |
					Club |
					Crossbow_Hand |
					Crossbow_Heavy |
					Crossbow_Light |
					Dagger |
					Dart |
					Flail |
					Glaive |
					Greataxe |
					Greatclub |
					Greatsword |
					Halberd |
					Handaxe |
					Javelin |
					Lance |
					LightCrossbow |
					LightHammer |
					Longbow |
					Longsword |
					Mace |
					Maul |
					Morningstar |
					Net |
					Pike |
					Quarterstaff |
					Rapier |
					Scimitar |
					Shortbow |
					Shortsword |
					Sickle |
					Sling |
					Spear |
					Trident |
					UnarmedStrike |
					Warhammer |
					WarPick |
					Whip
	}
}
