using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum CreatureKinds
	{
		None = 0,
		Aberration = 1,
		Beast = 2,
		Celestial = 4,
		Construct = 8,
		Dragon = 16,
		Elemental = 32,
		Fey = 64,
		Fiend = 128,
		Giant = 256,
		Humanoid = 512,
		Monstrosity = 1024,
		Ooze = 2048,
		Plant = 4096,
		Undead = 8192,
		AllCreatures = 
			Aberration | 
			Beast | 
			Celestial | 
			Construct |
			Dragon |
			Elemental |
			Fey |
			Fiend |
			Giant |
			Humanoid |
			Monstrosity |
			Ooze |
			Plant |
			Undead
	}
}