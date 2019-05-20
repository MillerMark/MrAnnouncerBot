using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum CreatureSize
	{
		None = 0,
		Tiny = 1,       // 	2 1/2 by 2 1/2 ft.			(Imp, sprite)
		Small = 2,      //	5 by 5 ft.							(Giant rat, goblin)
		Medium = 4,     //	5 by 5 ft.							(Orc, werewolf)
		Large = 8,      //	10 by 10 ft.						(Hippogriff, ogre)
		Huge = 16,      //	15 by 15 ft.						(Fire giant, treant)
		Gargantuan = 32 //  20 by 20 ft. or larger	(Kraken, purple worm)
	}
}
